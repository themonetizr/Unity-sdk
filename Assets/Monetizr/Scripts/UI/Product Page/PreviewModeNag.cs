using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Monetizr.UI {
	public class PreviewModeNag : MonoBehaviour
	{
		public Text secondParagraph;
		private void Start()
		{
			if (!Application.isEditor)
			{
				Destroy(gameObject);
				return;
			}

			var settings = MonetizrClient.Instance.Settings;
			#if UNITY_IOS
			if (settings.useIosNativePlugin)
			{
				secondParagraph.text = "iOS native plugin is enabled - great!";
			}
			else
			{
				secondParagraph.text = "iOS native plugin is disabled. It is recommended to use native plugin for the smoothest experience.";
			}
			#elif UNITY_ANDROID
			if (settings.useAndroidNativePlugin)
			{
				secondParagraph.text = "Android native plugin is enabled - great!";
			}
			else
			{
				secondParagraph.text = "Android native plugin is disabled. It is recommended to use native plugin for the smoothest experience.";
			}
			#else
			if (settings.bigScreen)
			{
				secondParagraph.text = "Big Screen is enabled for non-mobile platform - great!";
			}
			else
			{
				secondParagraph.text = "For non mobile platforms it is recommended to enable Big Screen - a large screen optimized view with in-game checkout.";
			}
			#endif
		}
	}
}
