using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Monetizr.Dto;
//Used for WebGL
using UnityEngine;
using UnityEngine.Networking;
using Monetizr.UI;
using Monetizr.UI.Theming;
using Monetizr.Telemetry;
using Monetizr.Utility;

namespace Monetizr
{
    public delegate void MonetizrErrorDelegate(string msg);

    //public delegate void MonetizrPaymentDelegate(Payment payment);
    public class MonetizrMonoBehaviour : MonoBehaviour
    {
        [Header("Monetizr Unity Plugin 1.4.0")]
        [SerializeField]
        [Tooltip("This is your oAuth Access token, provided by Monetizr.")]
        private string _accessToken;
        [SerializeField]
        [Tooltip("Should be left as is, however power users are free to customize our UIs to fit their needs.")]
        private GameObject _uiPrefab;
        [SerializeField]
        [Tooltip("Should be left as is, however power users are free to customize our UIs to fit their needs.")]
        private GameObject _webViewPrefab;

        [Header("UGUI Look and Feel")]
        [SerializeField]
        [Tooltip("Customize the colors of the product page. Does not update during gameplay. Does not update " +
                 "theme for native plugin views.")]
        private ColorScheme _colorScheme;

        [SerializeField]
        [Tooltip("Optimize for larger screens, such as desktops or TVs.")]
        private bool _bigScreen = false;

        [SerializeField]
        [Tooltip("Customize the look of the big screen view.")]
        private BigScreenThemingSettings _bigScreenSettings;

        [Header("Advanced Settings")]
        [SerializeField]
        [Tooltip("If something goes wrong, this will show an in-game error message. Disable to only output errors to the console.")]
        private bool _showFullscreenAlerts = false;

        /// <summary>
        /// Functions subscribed to this delegate are called whenever something
        /// calls <see cref="ShowError(string)"/>.
        /// </summary>
        public MonetizrErrorDelegate MonetizrErrorOccurred;
        
        /// <summary>
        /// <para>Functions subscribed to this delegate are called when a user has successfully
        /// filled out the checkout form and has pressed purchase.</para>
        ///
        /// <para>Responsibility for handling the transaction is handed over to the game developer.
        /// Note: this operation does not time out, therefore it is required to always
        /// call <see cref="Payment.Finish()"/> to prevent deadlocks.</para>
        /// </summary>
        //public MonetizrPaymentDelegate MonetizrPaymentStarted;

        [SerializeField]
        //Disable warnings so for platforms where platform specific variables aren't used so a pointless
        //warning doesn't show up.
#pragma warning disable
        [Tooltip("On Android and iOS devices, our SDK provides an in-game web browser for checkout. If this is enabled, all platforms" +
            " (except for WebGL) will use Unity's Application.OpenURL(string url) instead.")]
        private bool _neverUseWebView = false;

        [SerializeField]
        [Tooltip("On Android, instead of using your game's activity for displaying Monetizr, display" +
                 "a native overlay instead. Will still display using UGUI in editor for testing purposes." +
                 " Requires extra setup - consult the documentation.")]
        private bool _useAndroidNativePlugin = true;
#pragma warning restore
        [SerializeField]
        [Tooltip("If this is off, product pages will load silently.")]
        private bool _showLoadingScreen = true;

        [SerializeField] [Tooltip("Prefer device language instead of English for Unity known locales")]
        private bool _useDeviceLanguage = true;
        
        [SerializeField] [Tooltip("Currently used only in Big Screen mode - show links to Monetizr privacy policy and terms of service")]
        private bool _showPolicyLinks = true;

        [Header("EXPERIMENTAL")] [SerializeField]
        private bool _useNewCheckout = false;
        [SerializeField]
        private bool _useTestPlayerId = false;
        
        private GameObject _currentPrefab;
        private MonetizrUI _ui;
        private string _baseUrl = "https://api3.themonetizr.com/api/";
        private string _language;
        private string _playerId;

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
            
            if(_useTestPlayerId) SetPlayerId("RequiredForVerification");
        }

        public void SetAccessToken(string newToken)
        {
            _accessToken = newToken;
        }

        /// <summary>
        /// Set a unique identifier for this player. This ID will be used for handling 
        /// claim orders and other personalized offers.
        /// </summary>
        /// <param name="newId"></param>
        public void SetPlayerId(string newId)
        {
            _playerId = newId;
        }

        private void OnApplicationQuit()
        {
            Telemetrics.RegisterSessionEnd();
        }

        private void CreateUIPrefab()
        {
            // In editor we still want to use UGUI, however let's not waste resources creating UI
            // that will be superseded by native views.
            #if UNITY_ANDROID && !UNITY_EDITOR
            if (_useAndroidNativePlugin)
            {
                return;
            }
            #endif
            
            //Note: this safeguard SHOULD work but I recall a time when it didn't :(
            //Something I did fixed it, though.
            //Oh, it's because I thought it was a static. It isn't. 1 prefab per behavior, not globally.
            if (_currentPrefab != null) return; //Don't create the UI twice, accidentally

            _currentPrefab = Instantiate(_uiPrefab, null, true);
            DontDestroyOnLoad(_currentPrefab);
            _ui = _currentPrefab.GetComponent<MonetizrUI>();
            
            var themables = _ui.GetComponentsInChildren<IThemable>(true);
            foreach (var i in themables)
            {
                if(_bigScreenSettings.ColoringAllowed(i))
                    i.Apply(_colorScheme);
                
                _bigScreenSettings.CheckAndApplySpriteOverrides(i);
            }

            var fontThemables = _ui.GetComponentsInChildren<ThemableFont>(true);
            foreach (var i in fontThemables)
            {
                _bigScreenSettings.ApplyFont(i);
            }

            //_ui.SetProductPageScale(_bigScreen ? 0.7f : 1f);
            _ui.SetBigScreen(_bigScreen);
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
        /// <param name="forceOpenUrl">Whether to use Unity native opening, irregardless of platform</param>
        public void OpenURL(string url, bool forceOpenUrl = false)
        {
            if (forceOpenUrl)
            {
                Application.OpenURL(url);
                return;
            }
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
    #if UNITY_ANDROID
            if (_useAndroidNativePlugin)
            {  
                return;
            }
    #endif
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
        /// Determined automatically from device language if native plugin is used.
        /// </summary>
        /// <param name="language">Language and region information, e.g en_US or lv_LV</param>
        public void SetLanguage(string language)
        {
            _language = language;
        }

        public string Language
        {
            get { return _language; }
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
        
        public bool PolicyLinksEnabled
        {
            get { return _showPolicyLinks; }
        }

        public bool BackButtonHasAction
        {
            get { return _ui.BackButtonHasAction();}
        }

        public bool AnyUiIsOpened
        {
            get { return _ui.AnyUIOpen(); }
        }

        public bool UseNewCheckout
        {
            get { return _useNewCheckout; }
        }

        public string PlayerId
        {
            get { return _playerId; }
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
        
        [ContextMenu("Restore black color scheme")]
        private void SetDefaultBlackColorScheme()
        {
            _colorScheme.SetDefaultBlackTheme();
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
                _language = _useDeviceLanguage ? LanguageHelper.Get2LetterISOCodeFromSystemLanguage() : "en_En";

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
#if UNITY_ANDROID && !UNITY_EDITOR
            if (_useAndroidNativePlugin)
            {
                ShowError("Native plugin does not support preloaded products, however they load much faster which negates the need for preloading.");
                return;
            }
#endif
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

        //TODO: TESTING STUFF
        private bool useLockedProduct = false;
        
        /// <summary>
        /// Asynchronously loads and shows a <see cref="Product"/> for a given <paramref name="tag"/>. 
        /// This will immediately show a loading screen, unless disabled.
        /// </summary>
        /// <param name="tag">Product to show</param>
        public void ShowProductForTag(string tag)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (!_useAndroidNativePlugin)
            {  
                StartCoroutine(_ShowProductForTag(tag));
                return;
            }
            AndroidJavaClass pluginClass = new AndroidJavaClass("com.themonetizr.monetizrsdk.MonetizrSdk");
            //AndroidJavaClass companionClass = new AndroidJavaClass("com.themonetizr.monetizrsdk.MonetizrSdk$Companion");
            //AndroidJavaObject companion = pluginClass.
            AndroidJavaObject companion = pluginClass.GetStatic<AndroidJavaObject>("Companion");
            companion.Call("setDebuggable", true);
            companion.Call("setDynamicApiKey", _accessToken);
            companion.Call("showProductForTag", tag, useLockedProduct, "unset");
            useLockedProduct = !useLockedProduct;
#else
            StartCoroutine(_ShowProductForTag(tag));
#endif
        }

        public void AllProducts(Action<List<ListProduct>> list)
        {
            var l = new List<ListProduct>();
            
            StartCoroutine(GetData<ProductListDto>("products", prod =>
            {
                if (prod == null)
                {
                    list(null);
                    return;
                }
                
                prod.array.ForEach(x =>
                {
                    l.Add(ListProduct.FromDto(x));
                });
                list(l);
            }));
        }

        public void AllProducts(Action<List<ListProduct>> list)
        {
            var l = new List<ListProduct>();
            
            Debug.Log("Getting data...");
            StartCoroutine(GetData<ProductListDto>("products", prod =>
            {
                Debug.Log("Data got...");
                if (prod == null)
                {
                    list(null);
                    return;
                }
                
                prod.array.ForEach(x =>
                {
                    l.Add(ListProduct.FromDto(x));
                });
                list(l);
            }));
        }

        private IEnumerator _ShowProductForTag(string tag)
        {
            if (string.IsNullOrEmpty(_language))
                _language = _useDeviceLanguage ? LanguageHelper.Get2LetterISOCodeFromSystemLanguage() : "en";

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
            client.timeout = 20;
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
            www.timeout = 20;
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
                    MonetizrClient.Instance.ShowError(!string.IsNullOrEmpty(response) ? e.Message : "No response to POST request");
                    url(null);
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
            client.timeout = 20;
            var operation = client.SendWebRequest();
            yield return operation;
            try
            {
                if (operation.isDone)
                {
                    var data = client.downloadHandler.text;
                    if (data.StartsWith("["))
                    {
                        string newJson = "{ \"array\": " + data + "}";
                        result(JsonUtility.FromJson<T>(newJson));
                        yield break;
                    }
                }
                result(JsonUtility.FromJson<T>(client.downloadHandler.text));
            }
            catch (Exception e)
            {
                MonetizrClient.Instance.ShowError(!string.IsNullOrEmpty(client.downloadHandler.text) ? "EXCEPTION CAUGHT: " + e.ToString() : "No response to GET request"); 
                result(null);
            }
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
            client.timeout = 20;
            var operation = client.SendWebRequest();
            yield return operation;
            result(client.downloadHandler.text);
        }

        public void PostObjectWithResponse<T>(string actionUrl, object request, Action<T> response) where T : class
        {
            var json = JsonUtility.ToJson(request);

            StartCoroutine(MonetizrClient.Instance.PostDataWithResponse(actionUrl, json, result =>
            {
                var responseString = result;
                try
                {
                    if (responseString != null)
                    {
                        var responseObject = JsonUtility.FromJson<T>(responseString);
                        response(responseObject);
                    }
                    else
                    {
                        response(null);
                    }
                }
                catch (Exception e)
                {
                    MonetizrClient.Instance.ShowError(!string.IsNullOrEmpty(responseString) ? "EXCEPTION CAUGHT: " + e.ToString() : "No response to POST request"); 
                    response(null);
                }
            }));
        }

        private UnityWebRequest GetWebClient(string actionUrl, string method)
        {
            string finalUrl = _baseUrl + actionUrl;
            var client = new UnityWebRequest(finalUrl, method);
            client.SetRequestHeader("Content-Type", "application/json");
            client.SetRequestHeader("Authorization", "Bearer " + _accessToken);
            client.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            client.timeout = 20;
            return client;
        }
        #endregion
    }
}
