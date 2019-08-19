using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Monetizr
{
    public class MonetizrUI : MonoBehaviour
    {
        public ProductPageScript ProductPage;
        public AlertPage AlertPage;
        public GameObject LoadingIndicator;

        public void SetProductPage(bool active)
        {
            ProductPage.gameObject.SetActive(active);
        }

        public void SetLoadingIndicator(bool active)
        {
            LoadingIndicator.SetActive(active);
        }
    }
}