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
        private bool _ready = false;
        public MonetizrUI ui;
        public Image ProductInfoImage;
        public Text HeaderText;
        public Text PriceText;
        public Text DescriptionText;
        public Image HorizontalProductInfoImage;
        public Text HorizontalHeaderText;
        public Text HorizontalPriceText;
        public Text HorizontalDescriptionText;
        public Image BackgroundImage;
        public Image HorizontalBackgroundImage;
        //public HorizontalLayoutGroup ImageButtons;
        public GameObject ImagesViewPort;
        public List<VariantsDropdown> Dropdowns;
        public CanvasGroup PageCanvasGroup;
        private ProductInfo _productInfo;
        private string _tag;
        Dictionary<string, List<string>> _productOptions;
        public Animator VerticalLayoutAnimator;
        public CanvasGroupFader VerticalLayoutFader;
        public Animator HorizontalLayoutAnimator;
        public CanvasGroupFader HorizontalLayoutFader;
        public Animator DarkenAnimator;
        public ImageViewer ImageViewer;
        public SelectionManager SelectionManager;

        private bool _portrait = true;
        private bool _isOpened = false;

        private void Start()
        {
            ui.ScreenOrientationChanged += SwitchLayout;
        }

        private void OnDestroy()
        {
            ui.ScreenOrientationChanged -= SwitchLayout;
        }

        public void Revert()
        {
            _ready = false;
            ImageViewer.RemoveImages();
            ShowMainLayout();
            ImageViewer.HideViewer();
            SelectionManager.HideSelection();
        }

        public bool IsOpen()
        {
            if (!_ready) return false;
            return PageCanvasGroup.alpha >= 0.01f;
        }

        public void Init(ProductInfo info, string tag)
        {
            _portrait = Utility.UIUtilityScript.IsPortrait();
            Revert();
            _productOptions = new Dictionary<string, List<string>>();
            DescriptionText.text = info.data.productByHandle.description;
            HorizontalDescriptionText.text = DescriptionText.text;
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
            HorizontalPriceText.text = PriceText.text;
            HeaderText.text = info.data.productByHandle.title;
            HorizontalHeaderText.text = HeaderText.text;
            InitImages(info.data.productByHandle.images);
        }

        public void CloseProductPage()
        {
            MonetizrClient.Instance.RegisterProductPageDismissed(_tag);
            ui.SetProductPage(false);
        }

        public void SwitchLayout(bool portrait)
        {
            _portrait = portrait;
            BackgroundImage.enabled = _portrait;
            HorizontalBackgroundImage.enabled = !_portrait;
            UpdateOpenedAnimator();
        }

        public void UpdateOpenedAnimator()
        {
            VerticalLayoutAnimator.SetBool("Opened", _portrait ? _isOpened : false);
            HorizontalLayoutAnimator.SetBool("Opened", _portrait ? false : _isOpened);
            VerticalLayoutFader.DoEase(0.4f, _portrait ? 1 : 0, true);
            HorizontalLayoutFader.DoEase(0.4f, _portrait ? 0 : 1, true);
        }

        public void ShowMainLayout()
        {
            _isOpened = true;
            UpdateOpenedAnimator();
            DarkenAnimator.SetBool("Darken", false);
        }

        public void HideMainLayout()
        {
            _isOpened = false;
            UpdateOpenedAnimator();
            DarkenAnimator.SetBool("Darken", true);
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

            var url = _productInfo.data.productByHandle.onlineStoreUrl;

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

                    MonetizrClient.Instance.OpenURL(url);
                MonetizrClient.Instance.RegisterClick();
                    //ui.SetProductPage(false); Do we really need to close the page when user checks out?
                }));

            }


        }
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

            //StartCoroutine(DownloadImage(images.edges.FirstOrDefault().node.transformedSrc, ProductInfoImage));
            int i = 0; //We treaat the first image differently
            _imagesToDownload = images.edges.Count;
            foreach (var img in images.edges)
            {
                //var uiImage = Instantiate(BigImage, ImagesViewPort.transform, false);
                //var button = uiImage.GetComponent<Button>();
                //button.onClick.AddListener(() => { BigImage.sprite = uiImage.sprite; });
                StartCoroutine(DownloadImage(img.node.transformedSrc, i == 0));
                i++;
            }
        }

        private int _imagesToDownload;
        private int _imagesDownloaded;

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
                HorizontalProductInfoImage.sprite = spriteToUse;
                BackgroundImage.color = Utility.UIUtilityScript.ColorFromSprite(spriteToUse);
                HorizontalBackgroundImage.color = BackgroundImage.color;
            }
            else
            {
                ImageViewer.AddImage(spriteToUse, false);
            }


            www.Dispose();
            www = null;
            //yield return new WaitForSeconds(1f); //Hmmm.
            _imagesDownloaded++;
            if (_imagesDownloaded >= _imagesToDownload)
            {
                ui.SetLoadingIndicator(false);
                _ready = true;
            }
        }
    }
}
