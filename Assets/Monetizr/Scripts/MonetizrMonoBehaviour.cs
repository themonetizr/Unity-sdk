using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Monetizr.Dto;
using Monetizr.Payments;
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

    public delegate void MonetizrOrderDelegate(Product p);

    //public delegate void MonetizrPaymentDelegate(Payment payment);
    public class MonetizrMonoBehaviour : MonoBehaviour
    {
        private MonetizrSettings _settings;
        
        /// <summary>
        /// Functions subscribed to this delegate are called whenever something
        /// calls <see cref="ShowError(string)"/>.
        /// </summary>
        public MonetizrErrorDelegate MonetizrErrorOccurred;

        /// <summary>
        /// Functions subscribed to this delegate are called whenever a successful order is done. Do note that
        /// at the moment this is supported by Big Screen, Android with UGUI and iOS (only with Apple Pay).
        /// </summary>
        public MonetizrOrderDelegate MonetizrOrderConfirmed;

        private GameObject _currentPrefab;
        private MonetizrUI _ui;
        private string _baseUrl = "https://api3.themonetizr.com/api/";
        private string _language;
        private string _playerId;
        private string _overrideAccessToken = "";

        #region Initialization and basic features

#if UNITY_IOS && !UNITY_EDITOR && MONETIZR_IOS_NATIVE
        [DllImport("__Internal")]
        extern static private void objCinitMonetizr(string token);

        [DllImport("__Internal")]
        extern static private void objCshowProductForTag(string tag);

        [DllImport("__Internal")]
        extern static private void objCinitMonetizrApplePay(string merchantId, string companyName);

        [DllImport("__Internal")]
        extern static private void objCsetMonetizrTestMode(bool on);

        public void iOSPluginError(string message) {
            ShowError(message);
        }
#endif

        internal void Init(MonetizrSettings settings)
        {
            _settings = settings;
            
            //Private because there is no need to be switching access token mid-session.
            //In fact, the access token assignment is redundant, as it is set in inspector
            DontDestroyOnLoad(gameObject);

#if UNITY_IOS && !UNITY_EDITOR && MONETIZR_IOS_NATIVE
            objCinitMonetizr(AccessToken);

            if(_settings.applePay)
            {
                objCinitMonetizrApplePay(_settings.applePayMerchantId, _settings.applePayCompanyName);
                if (_settings.applePayTestMode)
                    objCsetMonetizrTestMode(true);
            }
#endif

            CreateUIPrefab();

            Telemetrics.ResetTelemetricsFlags();
            Telemetrics.RegisterSessionStart();
            Telemetrics.SendDeviceInfo();
            
            SetPlayerId("AABB01010101NotSet");
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

        /// <summary>
        /// Override the currently set access token. Assign empty string or null to disable override.
        /// </summary>
        /// <param name="token"></param>
        public void SetAccessTokenOverride(string token)
        {
            _overrideAccessToken = token;
            
#if UNITY_IOS && !UNITY_EDITOR && MONETIZR_IOS_NATIVE
            objCinitMonetizr(AccessToken);
#endif
        }

        public string AccessToken
        {
            get { return string.IsNullOrEmpty(_overrideAccessToken) ? _settings.accessToken : _overrideAccessToken; }
        }

        public bool IsTestingMode()
        {
            return _settings.testingMode;
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
            if (_settings.useAndroidNativePlugin && !_settings.showFullscreenAlerts)
            {
                return;
            }
#endif
#if UNITY_IOS && !UNITY_EDITOR
            if (_settings.useIosNativePlugin && !_settings.showFullscreenAlerts)
            {
                return;
            }
#endif
            
            //Note: this safeguard SHOULD work but I recall a time when it didn't :(
            //Something I did fixed it, though.
            //Oh, it's because I thought it was a static. It isn't. 1 prefab per behavior, not globally.
            if (_currentPrefab != null) return; //Don't create the UI twice, accidentally

            _currentPrefab = Instantiate(_settings.uiPrefab, null, true);
            DontDestroyOnLoad(_currentPrefab);
            _ui = _currentPrefab.GetComponent<MonetizrUI>();
            
            var themables = _ui.GetComponentsInChildren<IThemable>(true);
            foreach (var i in themables)
            {
                if(_settings.bigScreenSettings.ColoringAllowed(i))
                    i.Apply(_settings.colorScheme);
                
                _settings.bigScreenSettings.CheckAndApplySpriteOverrides(i);
            }

            var fontThemables = _ui.GetComponentsInChildren<ThemableFont>(true);
            foreach (var i in fontThemables)
            {
                _settings.bigScreenSettings.ApplyFont(i);
            }
            
            _ui.SetBigScreen(_settings.bigScreen);
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
            if (_settings.useAndroidNativePlugin)
            {  
                return;
            } else {
                GameObject newWebView;
                if (WebViewController.Current)
                    newWebView = WebViewController.Current.gameObject;
                else
                    newWebView = Instantiate(_settings.webViewPrefab, null, false);
                var wvc = newWebView.GetComponent<WebViewController>();
                wvc.Init();
                wvc.OpenURL(url);
            }
#elif UNITY_IOS
            if (_settings.useIosNativePlugin)
            {  
                return;
            }
#else
            Application.OpenURL(url);
#endif
#elif UNITY_WEBGL && !UNITY_EDITOR
            //For WebGL, use a native plugin to open links in new tabs
            if(_settings.webGlNewTab) {
                openWindow(url);
            }
            else {
                Application.OpenURL(url);
            }
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
            Debug.LogError("MONETIZR: " + v);
            if(MonetizrErrorOccurred != null)
                MonetizrErrorOccurred(v);
            if (_settings.showFullscreenAlerts)
                _ui.AlertPage.ShowAlert(v);
        }

        public bool LoadingScreenEnabled()
        {
            return _settings.showLoadingScreen;
        }
        
        public bool PolicyLinksEnabled
        {
            get { return _settings.showPolicyLinks; }
        }

        public bool BackButtonHasAction
        {
            get { return _ui.BackButtonHasAction();}
        }

        public bool AnyUiIsOpened
        {
            get { return _ui.AnyUIOpen(); }
        }

        public string PlayerId
        {
            get { return _playerId; }
        }

        [ContextMenu("Restore dark color scheme")]
        private void SetDefaultDarkColorScheme()
        {
            _settings.colorScheme.SetDefaultDarkTheme();
        }
        
        [ContextMenu("Restore light color scheme")]
        private void SetDefaultLightColorScheme()
        {
            _settings.colorScheme.SetDefaultLightTheme();
        }
        
        [ContextMenu("Restore black color scheme")]
        private void SetDefaultBlackColorScheme()
        {
            _settings.colorScheme.SetDefaultBlackTheme();
        }

        public MonetizrSettings Settings { get { return _settings; } }
#endregion

#region Product loading

        /// <summary>
        /// Asynchronously receive a <see cref="Product"/> for a given <paramref name="tag"/>. 
        /// If the request fails, <paramref name="product"/> will return <see langword="null"/>.
        /// </summary>
        /// <param name="tag">Tag of product to obtain</param>
        /// <param name="product">Method to do when product is obtained.</param>
        public void GetProduct(string tag, Action<Product> product, bool locked = false)
        {
            StartCoroutine(_GetProduct(tag, product, locked));
        }

        private IEnumerator _GetProduct(string tag, Action<Product> product, bool locked = false)
        {
            if (string.IsNullOrEmpty(_language))
                _language = _settings.useDeviceLanguage ? LanguageHelper.Get2LetterISOCodeFromSystemLanguage() : "en_En";

            Dto.ProductInfo productInfo;
            string request = String.Format("products/tag/{0}?language={1}&size={2}", tag, _language, Utility.UIUtility.GetMinScreenDimension());
            yield return StartCoroutine(GetData<Dto.ProductInfo>(request, result =>
            {
                Product p;
                productInfo = result;
                try
                {
                    p = Product.CreateFromDto(productInfo.data, tag);
                    p.Locked = locked;
                    product(p);
                }
                catch(Exception e)
                {
                    if (e is NullReferenceException)
                        ShowError("No valid response - offer probably doesn't exist or invalid API token");
                    else
                        Debug.LogError("Error getting offer: " + e);

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
            if (_settings.useAndroidNativePlugin)
            {
                ShowError("Native plugin does not support preloaded offers, however they load much faster which negates the need for preloading.");
                return;
            }
#elif UNITY_IOS && !UNITY_EDITOR
            if (_settings.useIosNativePlugin)
            {
                ShowError("Native plugin does not support preloaded offers, however they load much faster which negates the need for preloading.");
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

        /// <summary>
        /// Asynchronously loads and shows a <see cref="Product"/> for a given <paramref name="tag"/>. 
        /// This will immediately show a loading screen, unless disabled.
        /// </summary>
        /// <param name="tag">Product to show</param>
        /// <param name="locked">Whether to display this offer as locked and disallow ordering</param>
        public void ShowProductForTag(string tag, bool locked = false)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (!_settings.useAndroidNativePlugin)
            {  
                StartCoroutine(_ShowProductForTag(tag));
                return;
            }
            try
            {
                AndroidJavaClass pluginClass = new AndroidJavaClass("com.themonetizr.monetizrsdk.MonetizrSdk");
                AndroidJavaObject companion = pluginClass.GetStatic<AndroidJavaObject>("Companion");
                companion.Call("setDebuggable", true);
                companion.Call("setDynamicApiKey", AccessToken);
                companion.Call("showProductForTag", tag, locked, _playerId);
            }
            catch (Exception e)
            {
                ShowError("Failed to display using native plugin. It has probably not been set up properly.\n" + e.Message);
            }
#elif UNITY_IOS && !UNITY_EDITOR && MONETIZR_IOS_NATIVE
            if(!_settings.useIosNativePlugin)
            {
                StartCoroutine(_ShowProductForTag(tag));
                return;
            }
            try
            {
                objCshowProductForTag(tag);
            }
            catch(Exception e)
            {
                ShowError("Failed to display using native plugin. It has probably not been set up properly.\n" + e.Message);
            }
#else
            StartCoroutine(_ShowProductForTag(tag, locked));
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

        private IEnumerator _ShowProductForTag(string tag, bool locked = false)
        {
            if (string.IsNullOrEmpty(_language))
                _language = _settings.useDeviceLanguage ? LanguageHelper.Get2LetterISOCodeFromSystemLanguage() : "en";

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
            }, locked);

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
            if (Application.internetReachability == NetworkReachability.NotReachable || string.IsNullOrEmpty(imageUrl))
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
        /// <param name="checkout">Method to do when URL is obtained</param>
        public void GetCheckoutData(Dto.VariantStoreObject request, Action<Dto.Checkout> checkout)
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
                            checkout(checkoutObject.data.checkoutCreate.checkout);
                        else
                            checkout(null);
                    }
                }
                catch (System.Exception e)
                {
                    MonetizrClient.Instance.ShowError(!string.IsNullOrEmpty(response) ? e.Message : "No response to POST request");
                    checkout(null);
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
                //Debug.Log(responseString);
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
            client.SetRequestHeader("Authorization", "Bearer " + AccessToken);
            client.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            client.timeout = 20;
            return client;
        }

        public void PollPaymentStatus(Payment payment, IPaymentHandler handler)
        {
            StartCoroutine(_PollPaymentStatus(payment, handler));
        }

        private IEnumerator _PollPaymentStatus(Payment payment, IPaymentHandler handler)
        {
            PaymentStatusResponse currentResponse = null;
            var data = new PaymentStatusPostData
            {
                checkoutId = payment.Checkout.Id
            };
            bool called = false;
            while (true)
            {
                if (!called)
                {
                    PostObjectWithResponse<PaymentStatusResponse>("products/paymentstatus", data, x => currentResponse = x);
                    called = true;
                }

                if (currentResponse != null)
                {
                    handler.GetResponse(currentResponse);
                    called = false;
                    currentResponse = null;
                    yield return new WaitForSeconds(2);
                }

                if (!handler.IsPolling())
                {
                    yield break;
                }
                yield return null;
            }
        }

        public void PollCheckoutStatus(Dto.Checkout checkout, Action<CheckoutStatus> status)
        {
            var request = new CheckoutStatusRequest
            {
                checkoutId = checkout.id
            };
            PostObjectWithResponse<CheckoutStatus>("products/checkoutstatus", request, status);
        }
#endregion
    }
}
