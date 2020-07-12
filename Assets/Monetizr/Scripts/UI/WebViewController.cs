using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Monetizr.UI
{
    public class WebViewController : MonoBehaviour
    {
        private static WebViewController _current; //Hacky way of detecting if a wvc is open
        public static WebViewController Current { get { return _current; } }

        public WebViewObject wvo;
        public RectTransform topBar;
        public Animator animator;

        private int _IsBackAvailableHash;

        private void Start()
        {
            _IsBackAvailableHash = Animator.StringToHash("IsBackAvailable");
            _current = this;
        }

        public static bool IsOpen()
        {
            return _current != null;
        }

        public void Init()
        {
            wvo.Init();
            wvo.SetVisibility(true);
            UpdateMargins();
        }

        public void OpenURL(string url)
        {
            wvo.LoadURL(url);
        }

        public void UpdateMargins()
        {
#if UNITY_2017_2_OR_NEWER
            float newHeight = 100 + ((float)(Screen.height - Screen.safeArea.yMax) * ((float)1280 / (float)Utility.UIUtility.GetMaxScreenDimension()));
            Vector2 newSize = topBar.sizeDelta;
            newSize.y = newHeight;
            topBar.sizeDelta = newSize;
#endif
            Rect r = Utility.UIUtility.GetScreenCoordinates(topBar);
            wvo.SetMargins(0, (int)r.height, 0, 0);
        }

        //Public because used by UI
        public void Close()
        {
            wvo.SetVisibility(false);
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            _current = null;
        }

        //Public because used by UI
        public void HandleBackButton()
        {
            if(wvo.CanGoBack())
            {
                Debug.Log("WebView going back");
                wvo.GoBack();
            }
            else
            {
                Debug.Log("WebView closed because no more back available");
                Close();
            }
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
                HandleBackButton();

            animator.SetBool(_IsBackAvailableHash, wvo.CanGoBack());
        }
    }
}