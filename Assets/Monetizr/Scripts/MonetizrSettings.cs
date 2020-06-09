using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monetizr.UI;
using Monetizr.UI.Theming;
using Monetizr.Telemetry;
using Monetizr.Utility;

namespace Monetizr
{
	public class MonetizrSettings : ScriptableObject {
		[SerializeField]
		public GameObject uiPrefab;
		[SerializeField]
		public GameObject webViewPrefab;

		[Tooltip("This is your oAuth Access token, provided by Monetizr.")]
		public string accessToken;
		
		//[Header("UGUI Look and Feel")]
		[Tooltip("Customize the colors of the product page. Does not update during gameplay. Does not update " +
		         "theme for native plugin views.")]
		public ColorScheme colorScheme;
		
		[Tooltip("Optimize for larger screens, such as desktops or TVs.")]
		public bool bigScreen = false;
		
		[Tooltip("Customize the look of the big screen view.")]
		public BigScreenThemingSettings bigScreenSettings;
		
		//[Header("Advanced Settings")]
		[Tooltip("If something goes wrong, this will show an in-game error message. Disable to only output errors to the console.")]
		public bool showFullscreenAlerts = false;
		
		//Disable warnings so for platforms where platform specific variables aren't used so a pointless
		//warning doesn't show up.
#pragma warning disable
		[Tooltip("On Android and iOS devices, our SDK provides an in-game web browser for checkout. If this is enabled, all platforms" +
		         " (except for WebGL) will use Unity's Application.OpenURL(string url) instead.")]
		public bool neverUseWebView = false;
		
		[Tooltip("If this is off, product pages will load silently.")]
		public bool showLoadingScreen = true;
		
		[Tooltip("Prefer device language instead of English for Unity known locales")]
		public bool useDeviceLanguage = true;
        
		[Tooltip("Currently used only in Big Screen mode - show links to Monetizr privacy policy and terms of service")]
		public bool showPolicyLinks = true;
        
		[Tooltip("Currently used only in Big Screen mode - use testing mode for payments, see Stripe documentation for more info")]
		public bool testingMode = true;

		//[Header("EXPERIMENTAL")]
		[Tooltip("On Android, instead of using your game's activity for displaying Monetizr, display" +
		         "a native overlay instead. Will still display using UGUI in editor for testing purposes." +
		         " Requires extra setup - consult the documentation.")]
		public bool useAndroidNativePlugin = false;

		public bool useIosNativePlugin = false;

		public bool iosAutoBridgingHeader = false;
		public bool iosAutoconfig = false;
#pragma warning restore

		public void SetPrefabs(GameObject uiPrefab, GameObject webViewPrefab)
		{
			if(uiPrefab == null) throw new NullReferenceException();
			if(webViewPrefab == null) throw new NullReferenceException();
			this.uiPrefab = uiPrefab;
			this.webViewPrefab = webViewPrefab;
		}
	}
}