using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Monetizr.UI {
	public class PressHandler : MonoBehaviour, IPointerDownHandler
	{
        private Button _btn;
        public ProductPageScript productPage;
        public CheckoutWindow checkoutWindow;

        private void Start()
        {
	        _btn = GetComponent<Button>();
            if(_btn == null)
            {
                Debug.LogWarning("PressHandler could not find Button component. Disabling!");
                enabled = false;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
		{
			if (_btn.IsInteractable())
			{
#if UNITY_WEBGL
				// If we sense a mobile device on WebGL with this wonderful method,
				// force OpenURL, because the jslib thingy does not work well with touch
				// This solution is the fruits of a gruesome 90 minute debugging session :)
				if(productPage != null)
					productPage.OpenShop(Screen.width < Screen.height);
				else if (checkoutWindow != null)
					checkoutWindow.PressLoadingContinueButton();
#else
				if(productPage != null)
					productPage.OpenShop();
				else if (checkoutWindow != null)
					checkoutWindow.PressLoadingContinueButton();
#endif
			}
		}
	}
}