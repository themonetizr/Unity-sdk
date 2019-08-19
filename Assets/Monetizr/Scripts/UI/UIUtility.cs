using UnityEngine;
using UnityEngine.UI;

namespace Monetizr.Utility
{
    public static class UIUtilityScript
    {
        public static void ShowCanvasGroup(ref CanvasGroup cg)
        {
            cg.alpha = 1;
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }

        public static void HideCanvasGroup(ref CanvasGroup cg)
        {
            cg.alpha = 0;
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }

        public static void OpenWebView(string url)
        {
            WebViewObject webViewObject = new GameObject("WebViewObject").AddComponent<WebViewObject>();
            webViewObject.LoadURL(url);
        }
    }
}

