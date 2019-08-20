using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Monetizr
{
    public class MonetizrUI : MonoBehaviour
    {
        public ProductPageScript ProductPage;
        public Animator ProductPageAnimator;
        public AlertPage AlertPage;
        public GameObject LoadingIndicator;
        public Animator LoadingIndicatorAnimator;

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
    }
}