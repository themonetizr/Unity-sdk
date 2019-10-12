using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices; //Used for WebGL
using UnityEngine;
using UnityEngine.Networking;
using Monetizr.UI;
using Monetizr.UI.Theming;
using Monetizr.Telemetry;

namespace Monetizr
{
    public class MonetizrMonoBehaviour : MonoBehaviour
    {
        [Header("Monetizr Unity Plugin 1.2.0")]
        [SerializeField]
        [Tooltip("This is your oAuth Access token, provided by Monetizr.")]
        private string _accessToken;
        [SerializeField]
        [Tooltip("Should be left as is, however power users are free to customize our UIs to fit their needs.")]
        private GameObject _uiPrefab;
        [SerializeField]
        [Tooltip("Should be left as is, however power users are free to customize our UIs to fit their needs.")]
        private GameObject _webViewPrefab;

        [Header("Look and Feel")]
        [SerializeField]
        [Tooltip("Customize the colors of the product page. Does not update during gameplay.")]
        private ColorScheme _colorScheme;
        [SerializeField]
        [Tooltip("Use this to reduce the size of the Monetizr overlay. 0.6 recommended for games on large screens, like desktops.")]
        private float _scale = 1f;

        [Header("Advanced Settings")]
        [SerializeField]
        [Tooltip("If something goes wrong, this will show an in-game error message. Disable to only output errors to the console.")]
        private bool _showFullscreenAlerts = false;

        public delegate void MonetizrErrorDelegate(string msg);
        /// <summary>
        /// Functions subscribed to this delegate are called whenever something
        /// calls <see cref="ShowError(string)"/>.
        /// </summary>
        public MonetizrErrorDelegate MonetizrErrorOccurred;

        [SerializeField]
        //Disable warnings so for platforms where WebView isn't used a pointless
        //warning doesn't show up.
#pragma warning disable
        [Tooltip("On Android and iOS devices, our SDK provides an in-game web browser for checkout. If this is enabled, all platforms" +
            " (except for WebGL) will use Unity's Application.OpenURL(string url) instead.")]
        private bool _neverUseWebView = false;
#pragma warning restore
        [SerializeField]
        [Tooltip("If this is off, product pages will load silently.")]
        private bool _showLoadingScreen = true;

        private GameObject _currentPrefab;
        private MonetizrUI _ui;
        private string _baseUrl = "https://api3.themonetizr.com/api/";
        private string _language;

        #region Initialization and basic features

        private void Start()
        {
            if (MonetizrClient.Instance != null)
                if(MonetizrClient.Instance.gameObject != gameObject)
                {
                    Destroy(gameObject);
                    return;
                }
                    
            Init(_accessToken);
        }

        private void Init(string accessToken)
        {
            //Private because there is no need to be switching access token mid-session.
            //In fact, the access token assignment is redundant, as it is set in inspector
            DontDestroyOnLoad(gameObject);
            CreateUIPrefab();
            SetAccessToken(accessToken);

            Telemetrics.ResetTelemetricsFlags();
            Telemetrics.RegisterSessionStart();
            Telemetrics.SendDeviceInfo();
        }

        public void SetAccessToken(string newToken)
        {
            _accessToken = newToken;
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

            _currentPrefab = Instantiate(_uiPrefab, null, true);
            DontDestroyOnLoad(_currentPrefab);
            _ui = _currentPrefab.GetComponent<MonetizrUI>();
            
            var themables = _ui.GetComponentsInChildren<IThemable>(true);
            foreach(var i in themables)
                i.Apply(_colorScheme);

            _ui.SetProductPageScale(_scale);
        }

        //Use the native WebGL plugin for handling new tab opening
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void openWindow(string url);
#endif
        /// <summary>
        /// Open a HTTP(s) URL with the preferred method for current platform. Mobile platforms will use <see cref="WebViewController"/>, 
        /// WebGL will use a jslib plugin to open link in new tab and other platforms will use Unity's <see cref="Application.OpenURL(string)"/>
        /// </summary>
        /// <param name="url">HTTP(s) URL to open, Don't try mailto or any other wild stuff, please.</param>
        public void OpenURL(string url)
        {
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
            if(!_neverUseWebView)
            {
                GameObject newWebView;
                if (WebViewController.Current)
                    newWebView = WebViewController.Current.gameObject;
                else
                    newWebView = Instantiate(_webViewPrefab, null, false);
                var wvc = newWebView.GetComponent<WebViewController>();
                wvc.Init();
                wvc.OpenURL(url);
            }
            else
            {
                Application.OpenURL(url);
            }
#elif UNITY_WEBGL && !UNITY_EDITOR
            //For WebGL, use a native plugin to open links in new tabs
            openWindow(url);
#else
            //For all other platforms, just use a native call to open a browser window.
            Application.OpenURL(url);
#endif
        }

        /// <summary>
        /// This sets the language/region used for API calls. Based on this, product information can be retrieved in various languages.
        /// </summary>
        /// <param name="language">Language and region information, e.g en_US or lv_LV</param>
        public void SetLanguage(string language)
        {
            _language = language;
        }

        /// <summary>
        /// Used by Monetizr functions to report errors. In editor errors are reported to console. Additionally <see cref="MonetizrErrorOccurred"/> 
        /// can be subscribed to, to display custom messages to users. If <see cref="ShowFullscreenAlerts"/> is <see langword="true"/> then a built-in 
        /// fullscreen message will appear as well.
        /// </summary>
        /// <param name="v">The error message to print, most built-in functions also print the stacktrace.</param>
        public void ShowError(string v)
        {
#if UNITY_EDITOR
            Debug.LogError(v);
#endif
            if(MonetizrErrorOccurred != null)
                MonetizrErrorOccurred(v);
            if (_showFullscreenAlerts)
                _ui.AlertPage.ShowAlert(v);
        }

        public bool LoadingScreenEnabled()
        {
            return _showLoadingScreen;
        }

        public bool BackButtonHasAction
        {
            get { return _ui.BackButtonHasAction();}
        }

        public bool AnyUiIsOpened
        {
            get { return _ui.AnyUIOpen(); }
        }

        [ContextMenu("Restore dark color scheme")]
        private void SetDefaultDarkColorScheme()
        {
            _colorScheme.SetDefaultDarkTheme();
        }
        
        [ContextMenu("Restore light color scheme")]
        private void SetDefaultLightColorScheme()
        {
            _colorScheme.SetDefaultLightTheme();
        }
#endregion

        #region Product loading

        /// <summary>
        /// Asynchronously receive a <see cref="Product"/> for a given <paramref name="tag"/>. 
        /// If the request fails, <paramref name="product"/> will return <see langword="null"/>.
        /// </summary>
        /// <param name="tag">Tag of product to obtain</param>
        /// <param name="product">Method to do when product is obtained.</param>
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
                        ShowError("Error getting product because malformed or no JSON was received.");
                    else
                        Debug.LogError("Error getting product: " + e);

                    product(null);
                }
            }));
        }

        /// <summary>
        /// Open a product view for a given <see cref="Product"/> <paramref name="p"/>. 
        /// This requires a <see cref="Product"/> that has been correctly created.
        /// </summary>
        /// <param name="p">Product to show</param>
        public void ShowProduct(Product p)
        {
            StartCoroutine(_ShowProduct(p));
            Telemetrics.RegisterFirstImpressionProduct();
        }

        private IEnumerator _ShowProduct(Product product)
        {
            //_ui.SetProductPage(true);
            _ui.SetLoadingIndicator(true);
            //_ui.ProductPage.SetBackgrounds(_portraitBackground.texture, _landscapeBackground.texture);
            //_ui.ProductPage.SetLogo(_logo);
            _ui.ProductPage.Init(product);
            yield return null;
        }

        /// <summary>
        /// Asynchronously loads and shows a <see cref="Product"/> for a given <paramref name="tag"/>. 
        /// This will immediately show a loading screen, unless disabled.
        /// </summary>
        /// <param name="tag">Product to show</param>
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

#endregion

        #region API requests
        /// <summary>
        /// Send a POST request to the Monetizr API, without expecting a response.
        /// </summary>
        /// <param name="actionUrl">URL for the POST action (appended to base API URL)</param>
        /// <param name="jsonData">Data to send, formatted as JSON</param>
        /// <returns></returns>
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

        /// <summary>
        /// Asynchronously download an image as a <see cref="Sprite"/> from an URL <paramref name="imageUrl"/> and pass it to 
        /// the method <paramref name="result"/>. If the image download failed, <paramref name="result"/> will be <see langword="null"/>.
        /// </summary>
        /// <param name="imageUrl">URL to the image</param>
        /// <param name="result">Method to do when <see cref="Sprite"/> is obtained.</param>
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

        /// <summary>
        /// Asynchronously get a URL for the product checkout page for a given product variant. <see cref="Dto.VariantStoreObject"/> is created 
        /// by <see cref="Product.GetCheckoutUrl(Product.Variant, Action{string})"/> which also calls this method. 
        /// If the URL is not obtained, returns <see langword="null"/>.
        /// </summary>
        /// <param name="request">The product variant for which to get URL</param>
        /// <param name="url">Method to do when URL is obtained</param>
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
                        var checkoutObject = JsonUtility.FromJson<Dto.CheckoutResponse>(response);
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

        /// <summary>
        /// Send a GET request to the Monetizr API. <typeparamref name="T"/> should be a class, containing fields expected 
        /// in the returned JSON. If the request fails, will return <see langword="null"/>.
        /// </summary>
        /// <typeparam name="T">Class that follows the structure of expected JSON</typeparam>
        /// <param name="actionUrl">URL for the GET action (appended to base API URL)</param>
        /// <param name="result">Method to do when <typeparamref name="T"/> is obtained.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Send a POST request to the Monetizr API, expecting a response. 
        /// If the request fails, will return <see langword="null"/>.
        /// </summary>
        /// <param name="actionUrl">URL for the POST action (appended to base API URL)</param>
        /// <param name="jsonData">Data to send, formatted as JSON</param>
        /// <param name="result">Method to do when <paramref name="result"/> is obtained.</param>
        /// <returns></returns>
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
            client.SetRequestHeader("Authorization", "Bearer " + _accessToken);
            client.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            return client;
        }
        #endregion
    }
}
