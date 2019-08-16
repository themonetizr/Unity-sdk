using Monetizr.Dto;
using PaperPlaneTools;
using System;
using System.Collections;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Monetizr
{
    public class MonetizrMonoBehaviour : MonoBehaviour
    {
        [Header("Monetizr Plugin Settings")]
        public string AccessToken;
        public Canvas RootCanvas; //EVENTUALLY REMOVE THIS
        //public ProductPageScript ProductPrefab;
        public GameObject ProductPrefab;
        public ProductPageScript HorizontalProductPrefab;

        private ProductPageScript _currentPage;
        private string _baseUrl = "https://api3.themonetizr.com/api/";
        private bool _sessionRegistered;
        private bool _firstImpressionRegistered;
        private bool _firstClickRegistered;
        private DateTime? _sessionStartTime;
        private DateTime? _firstImpression;
        private DateTime? _firstClickTime;
        private string _language;

        public void Init(string accessToken)
        {
            AccessToken = accessToken;
            DisableFlags();
            RegisterSessionStart();
            SendDeviceInfo();

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

        private string _tag;
        public void ShowProductForTag(string tag)
        {
            if (!CheckConnection())
                return;

            _tag = tag;
            if (_currentPage && _currentPage.gameObject)
            {
                DestroyImmediate(_currentPage.gameObject);
            }

            if (Screen.width > Screen.height)
                StartCoroutine(ShowHorizontalProductForTagEnumerator(_tag));
            else
                StartCoroutine(ShowProductForTagEnumerator(_tag));
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

        private void ShowError(string v)
        {
#if UNITY_EDITOR
            Debug.Log(v);
#endif
#if UNITY_ANDROID || UNITY_IOS
        new Alert("Error", "You need to connect to the internet in order to see the products.").
            SetPositiveButton("OK")
            .Show();

#endif

        }

        private IEnumerator ShowHorizontalProductForTagEnumerator(string tag)
        {
            if (string.IsNullOrEmpty(_language))
                _language = "en_En";

            ProductInfo productInfo;
            string request = String.Format("products/tag/{0}?language={1}&size={2}", tag, _language, GetScreenSize());
            yield return StartCoroutine(GetData<ProductInfo>(request, result =>
            {
                productInfo = result;
                productInfo.data.productByHandle.description = CleanDescription(productInfo.data.productByHandle.descriptionHtml);
                _currentPage = Instantiate(HorizontalProductPrefab, RootCanvas.transform, false);
                _currentPage.Init(productInfo, tag);
                if (_sessionStartTime.HasValue && !_firstImpressionRegistered)
                {
                    _firstImpression = _firstImpression ?? DateTime.UtcNow;
                    var timespan = new { first_impression_shown = (int)(_firstImpression.Value - _sessionStartTime.Value).TotalSeconds };
                    var jsonData = JsonUtility.ToJson(timespan);
                    StartCoroutine(PostData("telemetric/firstimpression", jsonData));
                    _firstImpressionRegistered = true;
                }
            }));
        }

        private string CleanDescription(string HTMLCode)
        {
            HTMLCode = HTMLCode.Replace("\n", " ");

            // Remove tab spaces
            HTMLCode = HTMLCode.Replace("\t", " ");

            // Remove multiple white spaces from HTML
            HTMLCode = Regex.Replace(HTMLCode, "\\s+", " ");

            // Remove HEAD tag
            HTMLCode = Regex.Replace(HTMLCode, "<head.*?</head>", ""
                                , RegexOptions.IgnoreCase | RegexOptions.Singleline);

            // Remove any JavaScript
            HTMLCode = Regex.Replace(HTMLCode, "<script.*?</script>", ""
              , RegexOptions.IgnoreCase | RegexOptions.Singleline);

            // Replace special characters like &, <, >, " etc.
            StringBuilder sbHTML = new StringBuilder(HTMLCode);
            // Note: There are many more special characters, these are just
            // most common. You can add new characters in this arrays if needed
            string[] OldWords = { "&nbsp;", "&amp;", "&quot;", "&lt;", "&gt;", "&reg;", "&copy;", "&bull;", "&trade;" };
            string[] NewWords = { " ", "&", "\"", "<", ">", "(R)", "(C)", "-", "TM" };
            for (int i = 0; i < OldWords.Length; i++)
            {
                sbHTML.Replace(OldWords[i], NewWords[i]);
            }

            // Check if there are line breaks (<br>) or paragraph (<p>)
            sbHTML.Replace("<br>", Environment.NewLine + "<br>");
            sbHTML.Replace("<br ", Environment.NewLine + "<br ");
            sbHTML.Replace("<p", Environment.NewLine + "<p");
            sbHTML.Replace("<li", Environment.NewLine + "<li");


            var res = sbHTML.ToString();
            res = Regex.Replace(res, @"\p{Cs}", "");
            res = System.Text.RegularExpressions.Regex.Replace(res, "<[^>]*>", "");
            res = res.Replace(Environment.NewLine + " ", Environment.NewLine);
            // Finally, remove all HTML tags and return plain text
            return res.Trim();

        }

        private int GetScreenSize()
        {
            return Math.Min(Screen.height, Screen.width);
        }

        private IEnumerator ShowProductForTagEnumerator(string tag)
        {
            if (string.IsNullOrEmpty(_language))
                _language = "en_En";

            ProductInfo productInfo;
            string request = String.Format("products/tag/{0}?language={1}&size={2}", tag, _language, GetScreenSize());
            yield return StartCoroutine(GetData<ProductInfo>(request, result =>
            {
                productInfo = result;
                productInfo.data.productByHandle.description = CleanDescription(productInfo.data.productByHandle.descriptionHtml);
                GameObject newProduct = Instantiate(ProductPrefab, null, false);
                _currentPage = newProduct.GetComponent<ProductPageScript>();
                _currentPage.Init(productInfo, tag);
                if (_sessionStartTime.HasValue && !_firstImpressionRegistered)
                {
                    _firstImpression = _firstImpression ?? DateTime.UtcNow;
                    var timespan = new { first_impression_shown = (int)(_firstImpression.Value - _sessionStartTime.Value).TotalSeconds };
                    var jsonData = JsonUtility.ToJson(timespan);
                    StartCoroutine(PostData("telemetric/firstimpression", jsonData));
                    _firstImpressionRegistered = true;
                }
            }));

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

            var session = new SessionDto()
            {
                device_identifier = SystemInfo.deviceUniqueIdentifier,
                session_start = DateTime.UtcNow
            };

            _sessionStartTime = session.session_start;

            var jsonString = JsonUtility.ToJson(session);// JsonConvert.SerializeObject(session);
            StartCoroutine(PostData("telemetric/session", jsonString));
            _sessionRegistered = true;

        }

        private void Start()
        {
            //Assign itself to the singleton
            Init(AccessToken);
        }

        void OnApplicationQuit()
        {
            var session = new SessionDto()
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

            var deviceData = new DeviceData()
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


        private static IpInfo GetUserCountryByIp()
        {
#if UNITY_ANDROID || UNITY_IOS
        string IP = new WebClient().DownloadString("http://icanhazip.com");
        IpInfo ipInfo = new IpInfo();
        try
        {
            string info = new WebClient().DownloadString("http://ipinfo.io/" + IP);
            ipInfo = JsonUtility.FromJson<IpInfo>(info);// JsonConvert.DeserializeObject<IpInfo>(info);
            RegionInfo myRI1 = new RegionInfo(ipInfo.country);
            var ci = CultureInfo.CreateSpecificCulture(myRI1.TwoLetterISORegionName);
            ipInfo.region = $"{ci.TwoLetterISOLanguageName}-{myRI1.TwoLetterISORegionName}";
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
                yield return new WaitForSeconds(0);


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
