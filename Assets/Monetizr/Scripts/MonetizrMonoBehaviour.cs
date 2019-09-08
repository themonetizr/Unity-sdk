using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;
using Monetizr.UI;
using Monetizr.Telemetry;

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
        private string _language;

        public void SetLanguage(string language)
        {
            _language = language;
        }

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

            Telemetrics.ResetTelemetricsFlags();
            Telemetrics.RegisterSessionStart();
            Telemetrics.SendDeviceInfo();
        }

        private void OnApplicationQuit()
        {
            Telemetrics.RegisterSessionEnd();
        }

        private void CreateUIPrefab()
        {
            //Note: this safeguard SHOULD work but I recall a time when it didn't :(
            //Something I did fixed it, though.
            //Oh, it's because I thought it was a static. It isn't. 1 prefab per behavior, not globally.
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

        public void GetProduct(string tag, Action<Product> product)
        {
            StartCoroutine(_GetProduct(tag, product));
        }

        private IEnumerator _GetProduct(string tag, Action<Product> product)
        {
            if (string.IsNullOrEmpty(_language))
                _language = "en_En";

            Dto.ProductInfo productInfo;
            string request = String.Format("products/tag/{0}?language={1}&size={2}", tag, _language, Utility.UIUtility.GetMinScreenDimension());
            yield return StartCoroutine(GetData<Dto.ProductInfo>(request, result =>
            {
                Product p;
                productInfo = result;
                try
                {
                    p = Product.CreateFromDto(productInfo.data, tag);
                    product(p);
                }
                catch(Exception e)
                {
                    if (e is NullReferenceException)
                        Debug.LogError("Error getting product because malformed or no JSON was received.");
                    else
                        Debug.LogError("Error getting product: " + e);

                    product(null);
                }
            }));
        }

        public void ShowProduct(Product p)
        {
            StartCoroutine(_ShowProduct(p));
            Telemetrics.RegisterFirstImpressionProduct();
        }

        private IEnumerator _ShowProduct(Product product)
        {
            _ui.SetProductPage(true);
            _ui.SetLoadingIndicator(true);
            _ui.ProductPage.SetBackgrounds(PortraitBackground.texture, LandscapeBackground.texture, PortraitVideo, LandscapeVideo);
            _ui.ProductPage.SetLogo(Logo);
            _ui.ProductPage.Init(product);
            yield return null;
        }

        public void ShowProductForTag(string tag)
        {
            StartCoroutine(_ShowProductForTag(tag));
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
                }
                else
                {
                    FailLoading();
                }
            });

            yield return null;
        }

        private void FailLoading()
        {
            _ui.SetLoadingIndicator(false);
            _ui.SetProductPage(false);
        }

        public IEnumerator PostData(string actionUrl, string jsonData)
        {
            //Fail silently where nothing is expected to return
            if (Application.internetReachability == NetworkReachability.NotReachable)
                yield break;

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
                //ShowError("Could not download image, check network connection.");
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
            {
                //ShowError("No Internet connection");
                result(null);
                yield break;
            }

            var client = GetWebClient(actionUrl, "GET");
            var operation = client.SendWebRequest();
            yield return operation;
            if (operation.isDone)
                result(JsonUtility.FromJson<T>(client.downloadHandler.text));
        }

        public IEnumerator PostDataWithResponse(string actionUrl, string jsonData, Action<string> result)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                //ShowError("No Internet connection");
                result(null);
                yield break;
            }

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
