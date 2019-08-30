using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Monetizr
{
    public class MonetizrUI : MonoBehaviour
    {
        public delegate void MonetizrScreenOrientationDelegate(bool portrait);
        public MonetizrScreenOrientationDelegate ScreenOrientationChanged;
        private bool _lastOrientation;

        public ProductPageScript ProductPage;
        public Animator ProductPageAnimator;
        public AlertPage AlertPage;
        public GameObject LoadingIndicator;
        public Animator LoadingIndicatorAnimator;

        private void Start()
        {
            _lastOrientation = Utility.UIUtilityScript.IsPortrait();
        }

        public void SetProductPage(bool active)
        {
            //ProductPage.gameObject.SetActive(active);
            ProductPageAnimator.SetBool("Opened", active);
        }

        public void SetLoadingIndicator(bool active)
        {
            //LoadingIndicator.SetActive(active);
            LoadingIndicatorAnimator.SetBool("Opened", active);
        }

        /// <summary>
        /// Function for application developers to know whether Monetizr UI expects
        /// back button actions. Otherwise simultaneously active UIs can both read the back button.
        /// </summary>
        /// <returns>If back button is supposed to do something for Monetizr UI</returns>
        public bool HasBackButtonAction()
        {
            if(WebViewController.IsOpen())
            {
                return true;
            }
            if (AlertPage.IsOpen())
            {
                return true;
            }
            if (ProductPage.IsOpen())
            {
                return true;
            }
            return false;
        }

        public void HandleBackButton(bool fromSwipe = false)
        {
            if (WebViewController.IsOpen())
            {
                //WebViewController handles back button internally, so we ignore the rest.
                return;
            }
            if (AlertPage.IsOpen())
            {
                AlertPage.HideAlert();
                return;
            }
            if(ProductPage.IsOpen())
            {
                if (ProductPage.ImageViewer.IsOpen())
                {
                    ProductPage.ImageViewer.HideViewer();
                    return;
                }
                if(ProductPage.SelectionManager.IsOpen())
                {
                    ProductPage.SelectionManager.HideSelection();
                    return;
                }
                if (fromSwipe) return;
                ProductPage.CloseProductPage();
            }
        }

        public void HandleRightSwipe()
        {
            if (ProductPage.IsOpen())
            {
                if (ProductPage.SelectionManager.IsOpen())
                {
                    ProductPage.SelectionManager.PreviousSelect();
                }
            }
        }

        public void HandleLeftSwipe()
        {
            if (ProductPage.IsOpen())
            {
                if (ProductPage.SelectionManager.IsOpen())
                {
                    ProductPage.SelectionManager.NextSelect();
                }
            }
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                HandleBackButton();
            }

            bool thisOrientation = Utility.UIUtilityScript.IsPortrait();
            if(_lastOrientation != thisOrientation)
            {
                //Send a trigger to update layouts on screen orientation change.
                //ProductPageScript definitely subscribes to this.
                if (ScreenOrientationChanged != null)
                    ScreenOrientationChanged(thisOrientation);
            }
            _lastOrientation = thisOrientation;
        }
    }
}