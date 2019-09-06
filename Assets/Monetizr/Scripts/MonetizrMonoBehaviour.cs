using System;
using System.Collections;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using Monetizr.UI;

namespace Monetizr
{
    public class MonetizrMonoBehaviour : MonoBehaviour
    {
        [Header("Monetizr Plugin Settings")]
        public string AccessToken;
        public GameObject UIPrefab;
        public GameObject WebViewPrefab;

        [Header("Look and Feel")]
        public Sprite Logo;
        public Sprite PortraitBackground;
        public Sprite LandscapeBackground;
        [Header("or video background")]
        public VideoClip PortraitVideo;
        public VideoClip LandscapeVideo;

        [Header("Advanced Settings")]
        public bool ShowFullscreenAlerts = false;

        public delegate void MonetizrErrorDelegate(string msg);
        public MonetizrErrorDelegate MonetizrErrorOccurred;

        public bool NeverUseWebView = false;
        public bool ShowLoadingScreen = true;

        private GameObject _currentPrefab;
        private MonetizrUI _ui;
        private string _baseUrl = "https://api3.themonetizr.com/api/";
        private bool _sessionRegistered;
        private bool _firstImpressionRegistered;
        private bool _firstClickRegistered;
        private DateTime? _sessionStartTime;
        private DateTime? _firstImpression;
        private DateTime? _firstClickTime;
        private string _language;

        private void Start()
        {
            if (MonetizrClient.Instance != null)
                if(MonetizrClient.Instance.gameObject != gameObject)
                {
                    Destroy(gameObject);
                    return;
                }
                    
            Init(AccessToken);
        }

        public void Init(string accessToken)
        {
            DontDestroyOnLoad(gameObject);
            CreateUIPrefab();

            AccessToken = accessToken;
            DisableFlags();
            RegisterSessionStart();
            SendDeviceInfo();
        }

        private void CreateUIPrefab()
        {
            //Note: this safeguard didnt actually work for some reason.
            if (_currentPrefab != null) return; //Don't create the UI twice, accidentally

            _currentPrefab = Instantiate(UIPrefab, null, true);
            DontDestroyOnLoad(_currentPrefab);
            _ui = _currentPrefab.GetComponent<MonetizrUI>();
        }

        public void OpenURL(string url)
        {
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
            if(!NeverUseWebView)
            {
                GameObject newWebView;
                if (WebViewController.Current)
                    newWebView = WebViewController.Current.gameObject;
                else
                    newWebView = Instantiate(WebViewPrefab, null, false);
                var wvc = newWebView.GetComponent<WebViewController>();
                wvc.Init();
                wvc.OpenURL(url);
            }
            else
            {
                Application.OpenURL(url);
            }
#else
            //WebGL WebView implementations are finicky, it's easier to just open a new tab.
            Application.OpenURL(url);
#endif
        }

        internal void RegisterProductPageDismissed(string tag)
        {
            var value = new { trigger_tag = tag };
            var jsonData = JsonUtility.ToJson(value);
            StartCoroutine(PostData("telemetric/dismiss", jsonData));
        }

        internal void RegisterClick()
        {
            if (_firstClickRegistered || !_firstImpression.HasValue)
                return;

            _firstClickTime = _firstClickTime ?? DateTime.UtcNow;
            var timespan = new { first_impression_checkout = (int)(_firstClickTime.Value - _firstImpression.Value).TotalSeconds };
            var jsonData = JsonUtility.ToJson(timespan);
            StartCoroutine(PostData("telemetric/firstimpressioncheckout", jsonData));
            _firstClickRegistered = true;

        }

        public void SetLanguage(string language)
        {
            _language = language;
        }

        

        private bool CheckConnection()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                ShowError("Error. Check internet connection!");
                return false;
            }

            return true;
        }

        public void ShowError(string v)
        {
#if UNITY_EDITOR
            Debug.LogError(v);
#endif
            if(MonetizrErrorOccurred != null)
                MonetizrErrorOccurred(v);
            if (ShowFullscreenAlerts)
                _ui.AlertPage.ShowAlert(v);
        }

        private int GetScreenSize()
        {
            return Math.Min(Screen.height, Screen.width);
        }

        public void GetProduct(string tag, Action<Product> product)
        {
            StartCoroutine(_GetProduct(tag, product));
        }

        private IEnumerator _GetProduct(string tag, Action<Product> product)
        {
            if (string.IsNullOrEmpty(_language))
                _language = "en_En";

            Dto.ProductInfo productInfo;
            string request = String.Format("products/tag/{0}?language={1}&size={2}", tag, _language, GetScreenSize());
            yield return StartCoroutine(GetData<Dto.ProductInfo>(request, result =>
            {
                Product p;
                productInfo = result;
                try
                {
                    p = Product.CreateFromDto(productInfo.data, _tag);
                    product(p);
                }
                catch(Exception e)
                {
                    Debug.LogError("Error getting product: " + e);
                    product(null);
                }
            }));
        }

        public void ShowProduct(Product p)
        {
            StartCoroutine(_ShowProduct(p));
        }

        private IEnumerator _ShowProduct(Product product)
        {
            //Show the loading indicator IMMEDIATELY.
            //GameObject newProduct = Instantiate(UIPrefab, null, false);
            _ui.SetProductPage(true);
            _ui.SetLoadingIndicator(true);
            _ui.ProductPage.SetBackgrounds(PortraitBackground.texture, LandscapeBackground.texture, PortraitVideo, LandscapeVideo);
            _ui.ProductPage.SetLogo(Logo);
            _ui.ProductPage.Init(product);
            yield return null;
        }

        private string _tag;
        public void ShowProductForTag(string tag)
        {
            if (!CheckConnection())
                return;

            _tag = tag;
            StartCoroutine(_ShowProductForTag(_tag));
        }

        private IEnumerator _ShowProductForTag(string tag)
        {
            if (string.IsNullOrEmpty(_language))
                _language = "en_En";

            _ui.SetLoadingIndicator(true);

            GetProduct(tag, (p) =>
            {
                if(p != null)
                {
                    ShowProduct(p);

                    if (_sessionStartTime.HasValue && !_firstImpressionRegistered)
                    {
                        _firstImpression = _firstImpression ?? DateTime.UtcNow;
                        var timespan = new { first_impression_shown = (int)(_firstImpression.Value - _sessionStartTime.Value).TotalSeconds };
                        var jsonData = JsonUtility.ToJson(timespan);
                        StartCoroutine(PostData("telemetric/firstimpression", jsonData));
                        _firstImpressionRegistered = true;
                    }
                }
            });

            yield return null;
        }

        public void FailLoading()
        {
            _ui.SetLoadingIndicator(false);
            _ui.SetProductPage(false);
        }

        public void RegisterEncounter(string trigger_type = null, int? completion_status = null, string trigger_tag = null, string level_name = null, string difficulty_level_name = null, int? difficulty_estimation = null)
        {
            if (string.IsNullOrEmpty(level_name))
                level_name = SceneManager.GetActiveScene().name;

            var encounter = new { trigger_type, completion_status, trigger_tag, level_name, difficulty_level_name, difficulty_estimation };
            var jsonData = JsonUtility.ToJson(encounter);
            StartCoroutine(PostData("telemetric/encounter", jsonData));
        }

        private void ChangeOrientationTemplate()
        {
            ShowProductForTag(_tag);
        }

        private void RegisterSessionStart()
        {
            if (_sessionRegistered)
                return;

            var session = new Dto.SessionDto()
            {
                device_identifier = SystemInfo.deviceUniqueIdentifier,
                session_start = DateTime.UtcNow
            };

            _sessionStartTime = session.session_start;

            var jsonString = JsonUtility.ToJson(session);// JsonConvert.SerializeObject(session);
            StartCoroutine(PostData("telemetric/session", jsonString));
            _sessionRegistered = true;

        }

        void OnApplicationQuit()
        {
            var session = new Dto.SessionDto()
            {
                device_identifier = SystemInfo.deviceUniqueIdentifier,
                session_start = _sessionStartTime ?? DateTime.UtcNow,
                session_end = DateTime.UtcNow
            };

            var jsonString = JsonUtility.ToJson(session);// JsonConvert.SerializeObject(session);
            StartCoroutine(PostData("telemetric/session_end", jsonString));

            DisableFlags();

        }

        private void DisableFlags()
        {
            _firstImpressionRegistered = false;
            _sessionRegistered = false;
            _firstClickRegistered = false;
        }

        private void SendDeviceInfo()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
                return;

            var deviceData = new Dto.DeviceData()
            {
                language = Application.systemLanguage.ToString(),
                device_name = SystemInfo.deviceModel,
                device_identifier = SystemInfo.deviceUniqueIdentifier,
                os_version = SystemInfo.operatingSystem,
                region = GetUserCountryByIp().region
            };

            var jsonString = JsonUtility.ToJson(deviceData);// JsonConvert.SerializeObject(deviceData);
            StartCoroutine(PostData("telemetric/devicedata", jsonString));
        }


        private static Dto.IpInfo GetUserCountryByIp()
        {
#if UNITY_ANDROID || UNITY_IOS
        string IP = new WebClient().DownloadString("http://icanhazip.com");
        Dto.IpInfo ipInfo = new Dto.IpInfo();
        try
        {
            string info = new WebClient().DownloadString("http://ipinfo.io/" + IP);
            ipInfo = JsonUtility.FromJson<Dto.IpInfo>(info);// JsonConvert.DeserializeObject<IpInfo>(info);
            RegionInfo myRI1 = new RegionInfo(ipInfo.country);
            var ci = CultureInfo.CreateSpecificCulture(myRI1.TwoLetterISORegionName);
            ipInfo.region = ci.TwoLetterISOLanguageName + "-" + myRI1.TwoLetterISORegionName;
        }
        catch (Exception)
        {
            ipInfo.country = null;
        }

        return ipInfo;

#else
            return new IpInfo();
#endif

        }

        public IEnumerator PostData(string actionUrl, string jsonData)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
                yield return null; //WaitForSeconds(0) is not how you do stuff every frame
            //TODO: Also there should be a timeout for the requests.


            UnityWebRequest client = GetWebClient(actionUrl, "POST");

            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            client.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            var operation = client.SendWebRequest();
            yield return operation;
        }

        public void GetSprite(string imageUrl, Action<Sprite> result)
        {
            StartCoroutine(_GetSprite(imageUrl, result));
        }

        private IEnumerator _GetSprite(string imageUrl, Action<Sprite> image)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                ShowError("Could not download image, check network connection.");
                image(null); //We need to return null image to reset _downloadInProgress
                yield break;
            }

            // Start a download of the given URL
            var www = UnityWebRequestTexture.GetTexture(imageUrl);
            yield return www.SendWebRequest();

            if (www.isHttpError || www.isNetworkError)
            {
                ShowError(www.error);
                image(null);
                yield break;
            }

            // Create a texture in DXT1 format
            var texture = DownloadHandlerTexture.GetContent(www);

            Rect rec = new Rect(0, 0, texture.width, texture.height);
            Sprite finalSprite = Sprite.Create(texture, rec, new Vector2(0.5f, 0.5f), 100);
            www.Dispose();

            image(finalSprite);
        }

        public void GetCheckoutURL(Dto.VariantStoreObject request, Action<string> url)
        {
            var json = JsonUtility.ToJson(request);

            StartCoroutine(MonetizrClient.Instance.PostDataWithResponse("products/checkout", json, result =>
            {
                var response = result;
                try
                {
                    if (response != null)
                    {
                        var checkoutObject = JsonUtility.FromJson<Dto.CheckoutResponse>(response);// JsonConvert.DeserializeObject<CheckoutResponse>(response);
                        if (checkoutObject.data.checkoutCreate.checkoutUserErrors == null)
                            url(checkoutObject.data.checkoutCreate.checkout.webUrl);
                        else
                            url(null);
                    }
                }
                catch (System.Exception e)
                {
                    Debug.Log(e);
                    MonetizrClient.Instance.ShowError(e.Message + ": " + response ?? "No response");
                }
            }));
        }

        public IEnumerator GetData<T>(string actionUrl, Action<T> result) where T : class, new()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
                yield break;

            var client = GetWebClient(actionUrl, "GET");
            var operation = client.SendWebRequest();
            yield return operation;
            if (operation.isDone)
                result(JsonUtility.FromJson<T>(client.downloadHandler.text));
        }

        public IEnumerator PostDataWithResponse(string actionUrl, string jsonData, Action<string> result)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
                yield break;


            UnityWebRequest client = GetWebClient(actionUrl, "POST");

            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            client.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            var operation = client.SendWebRequest();
            yield return operation;

            result(client.downloadHandler.text);
        }

        private UnityWebRequest GetWebClient(string actionUrl, string method)
        {
            string finalUrl = _baseUrl + actionUrl;
            var client = new UnityWebRequest(finalUrl, method);
            client.SetRequestHeader("Content-Type", "application/json");
            client.SetRequestHeader("Authorization", "Bearer " + AccessToken);
            client.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            return client;
        }
    }
}
