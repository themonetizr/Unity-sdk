using Monetizr.Dto;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Monetizr.UI
{
    public class ProductPageScript : MonoBehaviour
    {
        public Product product;

        private bool _ready = false;
        public MonetizrUI ui;
        public List<ProductPageLayout> layouts;
        public List<VariantsDropdown> Dropdowns;
        public SelectionManager SelectionManager;
        public SelectionManagerBigScreen selectionManagerBigScreen;
        public CanvasGroup PageCanvasGroup;
        
        public Animator DarkenAnimator;
        public ImageViewer modalImageViewer;
        public GameObject outline;
        public Mask outlineMask;
        public Image maskImage;
        
        // Populate this list on start
        private List<ImageViewer> imageViewers = new List<ImageViewer>();
        private ProductInfo _productInfo;
        private string _tag;
        Dictionary<string, List<string>> _productOptions;
        
        private bool _portrait = true;
        private bool _isOpened = false;

        private float _checkoutUrlTimestamp = 0f;
        private string _currentCheckoutUrl = null;

        private float _heroImageTimestamp = 0f;
        private string _currentHeroImageUrl = null;
        private static readonly int Opened = Animator.StringToHash("Opened");

        // Used to hide variant selection when product has only one variant.
        private bool _singularVariant = false;

        public List<ImageViewer> ImageViewers
        {
            get { return imageViewers; }
        }

        private void Start()
        {
            ui.ScreenOrientationChanged += SwitchLayout;
            imageViewers.Add(modalImageViewer);
            layouts.ForEach(x =>
            {
                imageViewers.Add(x.imageViewer);
                for (int i = 0; i < Dropdowns.Count; i++)
                {
                    Dropdowns[i].Alternate.Add(x.alternateDropdowns[i].GetComponent<AlternateVariantsDropdown>());
                }
            });
        }

        private void OnDestroy()
        {
            ui.ScreenOrientationChanged -= SwitchLayout;
        }

        public void Revert()
        {
            _ready = false;
            foreach(var i in ImageViewers)
                i.RemoveImages();
            SwitchLayout(_portrait);
            ShowMainLayout();
            modalImageViewer.JumpToFirstImage();
            modalImageViewer.HideViewer();
            SelectionManager.HideSelection();
            selectionManagerBigScreen.HideSelection();
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
            _tag = p.Tag;
            var firstVariant = p.GetDefaultVariant();
            _singularVariant = p.Variants.Count <= 1;
            
            layouts.ForEach(x =>
            {
                x.description.text = p.Description;
                x.price.text = firstVariant.Price.FormattedPrice;
                x.header.text = p.Title;
                x.originalPriceBlock.SetActive(firstVariant.Price.Discounted);
                if (firstVariant.Price.Discounted)
                    x.originalPrice.text = firstVariant.Price.FormattedOriginalPrice;
                x.InitalizeDropdowns(_singularVariant);
            });
            
            SetCheckoutText(p.ButtonText);
            
            if (Dropdowns != null)
            {
                int i = 0;
                foreach (var dd in Dropdowns)
                {
                    dd.gameObject.SetActive(false);
                    dd.Init(new List<string>(), null, Dropdowns);
                }

                foreach (var option in p.Options)
                {
                    _productOptions.Add(option.Name, option.Options);

                    if (i < Dropdowns.Count)
                    {
                        var dd = Dropdowns.ElementAt(i);
                        dd.Init(option.Options, option.Name, Dropdowns);
                        if (!_singularVariant)
                        {
                            dd.gameObject.SetActive(true);
                            dd.Alternate.ForEach(x => x.gameObject.SetActive(true));
                        }

                        i++;
                    }
                }
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
                    foreach(var iView in ImageViewers)
                        iView.AddImage(imgs[i], true);
                    _heroImageTimestamp = Time.unscaledTime;
                    _currentHeroImageUrl = product.Images[0].Url;
                }
                else
                {
                    foreach(var iView in ImageViewers)
                        iView.AddImage(imgs[i], false);
                }
            }

            UpdateVariant();

            ui.SetLoadingIndicator(false);
            ui.SetProductPage(true);
            
            //Unity 2017.3->2018.2 report size 0 on Start, which means that we don't see images inline
            //We have to call the scaler somewhere in the middle to get around this.
            foreach(var iView in ImageViewers)
                iView.UpdateCellSize();
            
            _ready = true;
            layouts.ForEach(x => x.OnFinishedLoading());
            yield return null;
        }

        public void SetCheckoutText(string buttonText = "Purchase")
        {
            layouts.ForEach(x => x.checkoutText.text = buttonText);
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
            layouts.ForEach(x => x.ResetDescriptionPosition());
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

            layouts.ForEach(x =>
            {
                x.checkoutButton.interactable = false;
                x.checkoutText.text = selectedVariant != null ? "Please wait..." : "Sorry, this variant is unavailable!";
                x.price.text = (selectedVariant != null) ? selectedVariant.Price.FormattedPrice : "";
                if (selectedVariant == null)
                    x.originalPriceBlock.SetActive(false);
            });

            if(selectedVariant != null)
            {
                layouts.ForEach(x =>
                {
                    x.description.text = selectedVariant.Description;
                    x.originalPriceBlock.SetActive(selectedVariant.Price.Discounted);
                    if (selectedVariant.Price.Discounted)
                    {
                        x.originalPrice.text = selectedVariant.Price.FormattedOriginalPrice;
                    }
                });

                float currentTime = Time.unscaledTime;
                product.GetCheckoutUrl(selectedVariant, (url) =>
                {
                    if(currentTime > _checkoutUrlTimestamp)
                    {
                        _checkoutUrlTimestamp = currentTime;
                        layouts.ForEach(x =>
                        {
                            x.checkoutButton.interactable = true;
                            x.checkoutText.text = product.ButtonText;
                        });
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
                        layouts.ForEach(x => x.inlineImageLoaderAnimator.SetBool(Opened, true));
                        selectedVariant.Image.GetOrDownloadImage((img) =>
                        {
                            if(currentTime > _heroImageTimestamp)
                            {
                                _heroImageTimestamp = currentTime;
                                _currentHeroImageUrl = selectedVariant.Image.Url;
                                layouts.ForEach(x => x.inlineImageLoaderAnimator.SetBool(Opened, false));
                                //We also need to reset the image browser so that this is the first image
                                foreach(var iView in ImageViewers)
                                    iView.RemoveImages();
                                
                                Sprite[] imgs = product.GetAllImages();
                                for (int i = 0; i < imgs.Length; i++)
                                {
                                    if (i == 0)
                                    {
                                        foreach(var iView in ImageViewers)
                                            iView.AddImage(img, true);

                                        if(!product.Images[0].Url.Equals(_currentHeroImageUrl))
                                        {
                                            //If the base image and variant image are not the same
                                            //We need to add the base image to the viewer too
                                            foreach(var iView in ImageViewers)
                                                iView.AddImage(imgs[i], false);
                                        }
                                    }
                                    else
                                    {
                                        foreach(var iView in ImageViewers)
                                            iView.AddImage(imgs[i], false);
                                    }
                                }
                                
                                foreach(var iView in ImageViewers)
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
            //VerticalLayoutAnimator.SetBool("Opened", _portrait ? _isOpened : false);
            //HorizontalLayoutAnimator.SetBool("Opened", _portrait ? false : _isOpened);
            var requiredLayout = ProductPageLayout.Layout.None;
            if (!_isOpened)
            {
                // RequiredLayout stays none;
            }
            else if (ui.BigScreen)
                requiredLayout = ProductPageLayout.Layout.BigScreen;
            else if (_portrait)
                requiredLayout = ProductPageLayout.Layout.Vertical;
            else
                requiredLayout = ProductPageLayout.Layout.Horizontal;

            layouts.ForEach(x => x.OpenIfLayout(requiredLayout));
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
