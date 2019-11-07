using Monetizr.Dto;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Monetizr.UI
{
    public class ProductPageScript : MonoBehaviour
    {
        public Product product;

        private bool _ready = false;
        public MonetizrUI ui;
        public Text HeaderText;
        public Text PriceText;
        public Text DescriptionText;
        public GameObject originalPriceBlock;
        public Text originalPriceText;
        public Text HorizontalHeaderText;
        public Text HorizontalPriceText;
        public Text HorizontalDescriptionText;
        public GameObject horizontalOriginalPriceBlock;
        public Text horizontalOriginalPriceText;
        public Button[] CheckoutButtons;
        public Text[] CheckoutButtonTexts;
        public GameObject ImagesViewPort;
        public List<VariantsDropdown> Dropdowns;
        public List<GameObject> AlternateDropdowns;
        public CanvasGroup PageCanvasGroup;
        private ProductInfo _productInfo;
        private string _tag;
        Dictionary<string, List<string>> _productOptions;
        public Animator VerticalLayoutAnimator;
        public Animator HorizontalLayoutAnimator;
        public Animator DarkenAnimator;
        public ImageViewer modalImageViewer;
        public ImageViewer[] imageViewers;
        public SelectionManager SelectionManager;
        public Animator InlineImageLoaderAnimator;
        public Animator horizontalInlineImageLoaderAnimator;

        public RectTransform descriptionScroll;
        public RectTransform horizontalDescriptionScroll;

        public GameObject outline;
        public Mask outlineMask;
        public Image maskImage;

        private bool _portrait = true;
        private bool _isOpened = false;

        private float _checkoutUrlTimestamp = 0f;
        private string _currentCheckoutUrl = null;

        private float _heroImageTimestamp = 0f;
        private string _currentHeroImageUrl = null;
        private static readonly int Opened = Animator.StringToHash("Opened");

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
            foreach(var i in imageViewers)
                i.RemoveImages();
            SwitchLayout(_portrait);
            ShowMainLayout();
            modalImageViewer.JumpToFirstImage();
            modalImageViewer.HideViewer();
            SelectionManager.HideSelection();
        }

        public void SetOutline(bool state)
        {
            outline.SetActive(state);
            outlineMask.enabled = state;
            maskImage.enabled = state;
        }

        public bool IsOpen()
        {
            if (!_ready) return false;
            return PageCanvasGroup.alpha >= 0.01f;
        }

        public void Init(Product p)
        {
            product = p;
            _portrait = Utility.UIUtility.IsPortrait();
            Revert();
            _productOptions = new Dictionary<string, List<string>>();
            DescriptionText.text = p.Description;
            HorizontalDescriptionText.text = DescriptionText.text;
            SetCheckoutText(p.ButtonText);
            
            if (Dropdowns != null)
            {
                int i = 0;
                foreach (var dd in Dropdowns)
                {
                    dd.gameObject.SetActive(false);
                    dd.Init(new List<string>(), null, Dropdowns);
                }
                foreach (var dd in AlternateDropdowns)
                {
                    dd.SetActive(false);
                }

                foreach (var option in p.Options)
                {
                    _productOptions.Add(option.Name, option.Options);

                    if (i < Dropdowns.Count)
                    {
                        var dd = Dropdowns.ElementAt(i);
                        dd.Init(option.Options, option.Name, Dropdowns);
                        dd.gameObject.SetActive(true);

                        var add = AlternateDropdowns.ElementAt(i);
                        add.SetActive(true);
                        i++;
                    }
                }
            }
            _tag = p.Tag;
            var firstVariant = p.GetDefaultVariant();
            PriceText.text = firstVariant.Price.FormattedPrice;
            HorizontalPriceText.text = PriceText.text;
            HeaderText.text = p.Title;
            HorizontalHeaderText.text = HeaderText.text;
            originalPriceBlock.SetActive(firstVariant.Price.Discounted);
            horizontalOriginalPriceBlock.SetActive(firstVariant.Price.Discounted);
            if (firstVariant.Price.Discounted)
            {
                originalPriceText.text = firstVariant.Price.FormattedOriginalPrice;
                horizontalOriginalPriceText.text = originalPriceText.text;
            }
            p.DownloadAllImages();
            StartCoroutine(FinishLoadingProductPage());
        }

        IEnumerator FinishLoadingProductPage()
        {
            while (!product.AllImagesDownloaded)
            {
                int numDownloading = 0;
                foreach(var i in product.Images)
                {
                    if (i.IsDownloading) numDownloading++;
                }
                if(numDownloading == 0 && !product.AllImagesDownloaded)
                {
                    //The downloads have failed, abort mission!
                    MonetizrClient.Instance.ShowError("Failed to load product page, image download failed!");
                    ui.SetProductPage(false);
                    ui.SetLoadingIndicator(false);
                    yield break;
                }
                yield return null;
            }

            Revert();

            Sprite[] imgs = product.GetAllImages();
            for(int i=0;i<imgs.Length;i++)
            {
                if (i == 0)
                {
                    foreach(var iView in imageViewers)
                        iView.AddImage(imgs[i], true);
                    _heroImageTimestamp = Time.unscaledTime;
                    _currentHeroImageUrl = product.Images[0].Url;
                }
                else
                {
                    foreach(var iView in imageViewers)
                        iView.AddImage(imgs[i], false);
                }
            }

            UpdateVariant();

            ui.SetLoadingIndicator(false);
            ui.SetProductPage(true);
            
            //Unity 2017.3->2018.2 report size 0 on Start, which means that we don't see images inline
            //We have to call the scaler somewhere in the middle to get around this.
            foreach(var iView in imageViewers)
                iView.UpdateCellSize();
            
            _ready = true;

            yield return null;
        }

        public void SetCheckoutText(string buttonText = "Purchase")
        {
            foreach (var i in CheckoutButtonTexts)
            {
                i.text = buttonText;
            }
        }

        public void CloseProductPage()
        {
            Telemetry.Telemetrics.RegisterProductPageDismissed(_tag);
            ui.SetProductPage(false);
        }

        public void SwitchLayout(bool portrait)
        {
            _portrait = portrait;
            UpdateOpenedAnimator();
            Vector2 cur = descriptionScroll.anchoredPosition;
            cur.y = 0f;
            descriptionScroll.anchoredPosition = cur;
            cur = horizontalDescriptionScroll.anchoredPosition;
            cur.y = 0f;
            horizontalDescriptionScroll.anchoredPosition = cur;
        }

        public void UpdateVariant()
        {
            Product.Variant selectedVariant;
            Dictionary<string, string> currentSelection = new Dictionary<string, string>();

            foreach (var d in Dropdowns)
            {
                if(!string.IsNullOrEmpty(d.OptionName))
                    currentSelection[d.OptionName] = d.SelectedOption;
            }
            selectedVariant = product.GetVariant(currentSelection);

            foreach(var i in CheckoutButtons)
            {
                //Disable the checkout button, it will be later re-enabled by checkout link retrieval
                i.interactable = false;
            }

            foreach (var i in CheckoutButtonTexts)
            {
                i.text = (selectedVariant != null) ? "Please wait..." : "Sorry, this variant is unavailable!";
            }

            PriceText.text = (selectedVariant != null) ? selectedVariant.Price.FormattedPrice : "";
            HorizontalPriceText.text = PriceText.text;
            if (selectedVariant == null)
            {
                originalPriceBlock.SetActive(false);
                horizontalOriginalPriceBlock.SetActive(false);
            }

            if(selectedVariant != null)
            {
                DescriptionText.text = selectedVariant.Description;
                HorizontalDescriptionText.text = DescriptionText.text;
                
                originalPriceBlock.SetActive(selectedVariant.Price.Discounted);
                horizontalOriginalPriceBlock.SetActive(selectedVariant.Price.Discounted);
                if (selectedVariant.Price.Discounted)
                {
                    originalPriceText.text = selectedVariant.Price.FormattedOriginalPrice;
                    horizontalOriginalPriceText.text = originalPriceText.text;
                }

                float currentTime = Time.unscaledTime;
                product.GetCheckoutUrl(selectedVariant, (url) =>
                {
                    if(currentTime > _checkoutUrlTimestamp)
                    {
                        _checkoutUrlTimestamp = currentTime;
                        foreach (var i in CheckoutButtons)
                        {
                            //Fresh link obtained, enable the button back
                            i.interactable = true;
                        }
                        foreach (var i in CheckoutButtonTexts)
                        {
                            i.text = product.ButtonText;
                        }
                        _currentCheckoutUrl = url;
                    }
                });

                if(_ready)
                {
                    //If we are already seeing the product page, 
                    //update product images on variant change as well
                    //but only if it is different from what we are seeing now
                    if(!selectedVariant.Image.Url.Equals(_currentHeroImageUrl))
                    {
                        InlineImageLoaderAnimator.SetBool(Opened, true);
                        horizontalInlineImageLoaderAnimator.SetBool(Opened, true);
                        selectedVariant.Image.GetOrDownloadImage((img) =>
                        {
                            if(currentTime > _heroImageTimestamp)
                            {
                                _heroImageTimestamp = currentTime;
                                _currentHeroImageUrl = selectedVariant.Image.Url;
                                InlineImageLoaderAnimator.SetBool(Opened, false);
                                horizontalInlineImageLoaderAnimator.SetBool(Opened, false);
                                //We also need to reset the image browser so that this is the first image
                                foreach(var iView in imageViewers)
                                    iView.RemoveImages();
                                
                                Sprite[] imgs = product.GetAllImages();
                                for (int i = 0; i < imgs.Length; i++)
                                {
                                    if (i == 0)
                                    {
                                        foreach(var iView in imageViewers)
                                            iView.AddImage(img, true);

                                        if(!product.Images[0].Url.Equals(_currentHeroImageUrl))
                                        {
                                            //If the base image and variant image are not the same
                                            //We need to add the base image to the viewer too
                                            foreach(var iView in imageViewers)
                                                iView.AddImage(imgs[i], false);
                                        }
                                    }
                                    else
                                    {
                                        foreach(var iView in imageViewers)
                                            iView.AddImage(imgs[i], false);
                                    }
                                }
                                
                                foreach(var iView in imageViewers)
                                    iView.JumpToFirstImage();
                            }
                        });
                    }
                }
            }
            else
            {
                _checkoutUrlTimestamp = Time.unscaledTime;
            }
        }

        public void UpdateOpenedAnimator()
        {
            VerticalLayoutAnimator.SetBool("Opened", _portrait ? _isOpened : false);
            HorizontalLayoutAnimator.SetBool("Opened", _portrait ? false : _isOpened);
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

        public void OpenShop(bool forceOpenUrl = false)
        {
            if (!string.IsNullOrEmpty(_currentCheckoutUrl))
                MonetizrClient.Instance.OpenURL(_currentCheckoutUrl, forceOpenUrl);
        }
    }
}
