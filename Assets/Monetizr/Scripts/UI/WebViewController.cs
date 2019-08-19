using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Monetizr
{
    public class WebViewController : MonoBehaviour
    {
        public WebViewObject wvo; //WebViewObject TOO LONG.
        public RectTransform topBar;
        public Animator animator;

        private int _IsBackAvailableHash;

        private void Start()
        {
            _IsBackAvailableHash = Animator.StringToHash("IsBackAvailable");
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
            Rect r = Utility.UIUtilityScript.GetScreenCoordinates(topBar);
            wvo.SetMargins(0, (int)r.height, 0, 0);
        }

        //Public because used by UI
        public void Close()
        {
            wvo.SetVisibility(false);
            Destroy(gameObject);
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