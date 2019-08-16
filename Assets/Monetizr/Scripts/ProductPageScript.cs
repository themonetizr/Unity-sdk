using Monetizr.Dto;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Monetizr
{
    public class ProductPageScript : MonoBehaviour
    {
        public Image ProductInfoImage;
        //public HorizontalLayoutGroup ImageButtons;
        public GameObject ImagesViewPort;
        public Text HeaderText;
        public Text PriceText;
        public List<VariantsDropdown> Dropdowns;
        public Text DescriptionText;
        public GameObject BusyIndicator;
        private ProductInfo _productInfo;
        private string _tag;
        Dictionary<string, List<string>> _productOptions;
        public ImageViewer ImageViewer;

        public void Init(ProductInfo info, string tag)
        {
            _productOptions = new Dictionary<string, List<string>>();
            DescriptionText.text = info.data.productByHandle.description;
            var options = info.data.productByHandle.variants.edges.SelectMany(x => x.node.selectedOptions.Select(y => y.name)).Distinct().ToList();
            if (Dropdowns != null)
            {
                int i = 0;

                foreach (var dd in Dropdowns)
                {
                    dd.gameObject.SetActive(false);
                }

                foreach (var option in options)
                {
                    var possibleOptions = info.data.productByHandle.variants.edges
                        .SelectMany(x => x.node.selectedOptions.Select(y => y))
                        .Where(x => x.name == option)
                        .Select(x => x.value).Distinct().ToList();
                    _productOptions.Add(option, possibleOptions);

                    if (i < Dropdowns.Count)
                    {
                        var dd = Dropdowns.ElementAt(i);
                        dd.Init(possibleOptions, option, Dropdowns);
                        dd.gameObject.SetActive(true);
                        i++;
                    }
                }
            }
            _productInfo = info;
            _tag = tag;
            var firstVariant = info.data.productByHandle.variants.edges.FirstOrDefault();
            PriceText.text = firstVariant.node.priceV2.currencyCode + firstVariant.node.priceV2.amount;
            HeaderText.text = info.data.productByHandle.title;
            BusyIndicator.SetActive(true);
            InitImages(info.data.productByHandle.images);
        }

        public void CloseProductPage()
        {
            MonetizrClient.Instance.RegisterProductPageDismissed(_tag);
            Destroy(gameObject);
        }

        public void OpenShop()
        {
            VariantsEdge selectedEdge = null;
            foreach (var variant in _productInfo.data.productByHandle.variants.edges)
            {
                bool rightOption = true;
                foreach (var dd in Dropdowns)
                {
                    if (!dd.isActiveAndEnabled)
                        continue;

                    var option = variant.node.selectedOptions.FirstOrDefault(x => x.name == dd.OptionName);
                    if (option.value != dd.SelectedOption)
                        rightOption = false;
                }

                if (rightOption)
                    selectedEdge = variant;

                if (selectedEdge != null)
                    break;
            }

#pragma warning disable
            //This gives a warning because url is only used in builds, not editor play mode.
            var url = _productInfo.data.productByHandle.onlineStoreUrl;
#pragma warning restore

            if (selectedEdge == null)
                return;
            if(selectedEdge.node == null)
                return;
            //selectedEdge?.node?.id 
            if (selectedEdge.node.id != null)
            {
                var request = new VariantStoreObject()
                {
                    product_handle = _tag,
                    quantity = 1,
                    variantId = selectedEdge.node.id
                };
                var jsonData = JsonUtility.ToJson(request); //JsonConvert.SerializeObject(request);
                StartCoroutine(MonetizrClient.Instance.PostDataWithResponse("products/checkout", jsonData, result =>
                {
                    var response = result;
                    if (response != null)
                    {
                        var checkoutObject = JsonUtility.FromJson<CheckoutResponse>(response);// JsonConvert.DeserializeObject<CheckoutResponse>(response);
                    if (checkoutObject.data.checkoutCreate.checkoutUserErrors == null || !checkoutObject.data.checkoutCreate.checkoutUserErrors.Any())
                            url = checkoutObject.data.checkoutCreate.checkout.webUrl;
                    }

#if UNITY_IPHONE || UNITY_ANDROID
                InAppBrowser.OpenURL(url);
#endif
#if UNITY_WEBGL
                openWindow(url);
#endif
                MonetizrClient.Instance.RegisterClick();
                    Destroy(gameObject);
                }));

            }


        }

#if UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void openWindow(string url);
#endif
        private void InitImages(Images images)
        {
            //images?.edges?.FirstOrDefault()?.node?.transformedSrc
            //Much of this surely is redundant!
            if (images == null)
                return;
            if (images.edges == null)
                return;
            if (images.edges.FirstOrDefault() == null)
                return;
            if (images.edges.FirstOrDefault().node == null)
                return;
            if (images.edges.FirstOrDefault().node.transformedSrc == null)
                return;

            StartCoroutine(DownloadImage(images.edges.FirstOrDefault().node.transformedSrc, ProductInfoImage));
            int i = 0; //We treaat the first image differently
            foreach (var img in images.edges)
            {
                //var uiImage = Instantiate(BigImage, ImagesViewPort.transform, false);
                //var button = uiImage.GetComponent<Button>();
                //button.onClick.AddListener(() => { BigImage.sprite = uiImage.sprite; });
                StartCoroutine(DownloadImage(img.node.transformedSrc, i == 0));
                i++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="targetImage">null for adding image to ImageViewer</param>
        /// <returns></returns>
        IEnumerator DownloadImage(string url, bool first = false)
        {
            // Start a download of the given URL
            var www = UnityWebRequestTexture.GetTexture(url);

            yield return www.SendWebRequest();
            // Create a texture in DXT1 format
            var texture = DownloadHandlerTexture.GetContent(www);

            Rect rec = new Rect(0, 0, texture.width, texture.height);
            Sprite spriteToUse = Sprite.Create(texture, rec, new Vector2(0.5f, 0.5f), 100);
            //targetImage.sprite = spriteToUse;
            if(first)
            {
                ImageViewer.AddImage(spriteToUse, first);
                ProductInfoImage.sprite = spriteToUse;
            }
            else
            {
                ImageViewer.AddImage(spriteToUse, false);
            }


            www.Dispose();
            www = null;
            yield return new WaitForSeconds(1f); //Hmmm.
            BusyIndicator.SetActive(false);
        }
    }
}
