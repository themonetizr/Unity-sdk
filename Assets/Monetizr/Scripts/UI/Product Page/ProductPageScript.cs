using Monetizr.Dto;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Monetizr.UI
{
    public class ProductPageScript : MonoBehaviour
    {
        public Product product;

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
        public Image[] LogoImages;
        public RawImage BackgroundImage;
        public RawImage HorizontalBackgroundImage;
        public Button[] CheckoutButtons;
        public Text[] CheckoutButtonTexts;
        public RenderTexture VideoRenderTexture;
        public VideoPlayer BackgroundVideo;
        public VideoPlayer HorizontalBackgroundVideo;
        //public HorizontalLayoutGroup ImageButtons;
        public GameObject ImagesViewPort;
        public List<VariantsDropdown> Dropdowns;
        public List<GameObject> AlternateDropdowns;
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

        private float _checkoutUrlTimestamp = 0f;
        private string _currentCheckoutUrl = null;

        private float _heroImageTimestamp = 0f;
        private string _currentHeroImageUrl = null;

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
            SwitchLayout(_portrait);
            ShowMainLayout();
            ImageViewer.HideViewer();
            SelectionManager.HideSelection();
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
                }
                yield return null;
            }

            Revert();

            Sprite[] imgs = product.GetAllImages();
            for(int i=0;i<imgs.Length;i++)
            {
                if (i == 0)
                {
                    ImageViewer.AddImage(imgs[i], true);
                    ProductInfoImage.sprite = imgs[i];
                    HorizontalProductInfoImage.sprite = imgs[i];
                    //Disable background color changing for now.
                    //BackgroundImage.color = Utility.UIUtilityScript.ColorFromSprite(spriteToUse);
                    HorizontalBackgroundImage.color = BackgroundImage.color;
                    _heroImageTimestamp = Time.unscaledTime;
                    _currentHeroImageUrl = product.Images[0].Url;
                }
                else
                {
                    ImageViewer.AddImage(imgs[i], false);
                }
            }

            UpdateVariant();

            ui.SetLoadingIndicator(false);
            _ready = true;

            yield return null;
        }

        public void SetBackgrounds(Texture2D portrait = null, Texture2D landscape = null, VideoClip portraitVideo = null, VideoClip landscapeVideo = null)
        {
            BackgroundVideo.Stop();
            HorizontalBackgroundVideo.Stop();

            if (portraitVideo != null)
            {
                BackgroundImage.texture = VideoRenderTexture;
                BackgroundVideo.clip = portraitVideo;
            }
            else
            {
                BackgroundImage.texture = portrait;
            }

            if (landscapeVideo != null)
            {
                HorizontalBackgroundImage.texture = VideoRenderTexture;
                HorizontalBackgroundVideo.clip = landscapeVideo;
            }
            else
            {
                HorizontalBackgroundImage.texture = landscape;
            }

            UpdateBackgroundPlayback();
        }

        public void SetLogo(Sprite newLogo = null)
        {
            foreach(var i in LogoImages)
            {
                i.sprite = newLogo;
                i.enabled = (newLogo != null);
            }
        }

        public void SetCheckoutText(string buttonText = "Purchase")
        {
            foreach(var i in CheckoutButtonTexts)
            {
                i.text = buttonText;
            }
        }

        public void UpdateBackgroundPlayback()
        {
            if(Utility.UIUtility.IsPortrait())
            {
                if (BackgroundImage.texture == VideoRenderTexture)
                    BackgroundVideo.Play();
                if (HorizontalBackgroundImage.texture == VideoRenderTexture)
                    HorizontalBackgroundVideo.Stop();
            }
            else
            {
                if (BackgroundImage.texture == VideoRenderTexture)
                    BackgroundVideo.Stop();
                if (HorizontalBackgroundImage.texture == VideoRenderTexture)
                    HorizontalBackgroundVideo.Play();
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
            BackgroundImage.enabled = _portrait;
            HorizontalBackgroundImage.enabled = !_portrait;
            UpdateOpenedAnimator();
            UpdateBackgroundPlayback();
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

            if(selectedVariant != null)
            {
                DescriptionText.text = selectedVariant.Description;
                HorizontalDescriptionText.text = DescriptionText.text;

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
                    if(!selectedVariant.Image.Url.Equals(_currentHeroImageUrl))
                    {
                        selectedVariant.Image.GetOrDownloadImage((img) =>
                        {
                            if(currentTime > _heroImageTimestamp)
                            {
                                _heroImageTimestamp = currentTime;
                                _currentHeroImageUrl = selectedVariant.Image.Url;
                                ProductInfoImage.sprite = img;
                                HorizontalProductInfoImage.sprite = img;
                                //We also need to reset the image browser so that this is the first image
                                ImageViewer.RemoveImages();
                                Sprite[] imgs = product.GetAllImages();
                                for (int i = 0; i < imgs.Length; i++)
                                {
                                    if (i == 0)
                                    {
                                        ImageViewer.AddImage(img, true);
                                        if(!product.Images[0].Url.Equals(_currentHeroImageUrl))
                                        {
                                            //If the base image and variant image are not the same
                                            //We need to add the base image to the viewer too
                                            ImageViewer.AddImage(imgs[i], false);
                                        }
                                    }
                                    else
                                    {
                                        ImageViewer.AddImage(imgs[i], false);
                                    }
                                }
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
            /*Product.Variant selectedVariant;
            Dictionary<string, string> currentSelection = new Dictionary<string, string>();
            
            foreach(var d in Dropdowns)
            {
                currentSelection[d.OptionName] = d.SelectedOption;
            }
            selectedVariant = product.GetVariant(currentSelection) ?? product.GetDefaultVariant();

            product.GetCheckoutUrl(selectedVariant, (url) =>
            {
                if (!string.IsNullOrEmpty(url))
                    MonetizrClient.Instance.OpenURL(url);
            });*/

            if (!string.IsNullOrEmpty(_currentCheckoutUrl))
                MonetizrClient.Instance.OpenURL(_currentCheckoutUrl);
        }
    }
}
