using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Monetizr
{
    /// <summary>
    /// Contains all information required to display a product page with all functionality. 
    /// Also contains methods to aid with custom usage.
    /// </summary>
    public class Product
    {
        private static string CleanDescriptionIos(string d)
        {
            string desc_1 = d;
            //description_ios starts with a newline for whatever reason, so we get rid of that
            if (desc_1[0] == '\n')
                desc_1 = desc_1.Substring(1);

            //this regex removes emojis
            desc_1 = Regex.Replace(desc_1, @"\p{Cs}", "");

            //Regex is hard, let's do this in a garbage-generat-y way
            string desc_2 = "";
            foreach (string l in desc_1.Split('\n'))
            {
                desc_2 += l.Trim(' ', '\u00A0');
                desc_2 += '\n';
            }

            return desc_2;
        }

        /// <summary>
        /// Defines a variant group.
        /// </summary>
        public class Option
        {
            /// <summary>
            /// Name for the variant group (Size/Color/etc)
            /// </summary>
            public string Name;
            /// <summary>
            /// List of available options for this variant group.
            /// </summary>
            public List<string> Options;

            public Option()
            {
                Options = new List<string>();
            }

            public Option(string[] options)
            {
                Options = new List<string>(options);
            }
        }

        /// <summary>
        /// Defines a downloadable image for <see cref=">Product"/>. Can be used independently as well.
        /// </summary>
        public class DownloadableImage
        {
            /// <summary>
            /// URL for the image that will be downloaded
            /// </summary>
            public string Url;
            /// <summary>
            /// This <see cref="UnityEngine.Sprite"/> will hold the downloaded image
            /// </summary>
            public Sprite Sprite;
            /// <summary>
            /// Get whether this <see cref="DownloadableImage"/> already has a Sprite downloaded.
            /// </summary>
            public bool Downloaded
            {
                get
                {
                    return Sprite != null;
                }
            }

            public DownloadableImage()
            {

            }

            public DownloadableImage(string url)
            {
                Url = url;
            }

            /// <summary>
            /// Get whether this image is currently being downloaded.
            /// </summary>
            public bool IsDownloading
            {
                get
                {
                    return _downloadInProgress;
                }
            }
            private bool _downloadInProgress = false;
            /// <summary>
            /// Start downloading the image. You can check if image is downloaded with <see cref="Downloaded"/>.
            /// </summary>
            public void DownloadImage()
            {
                if (_downloadInProgress) return;
                _downloadInProgress = true;
                MonetizrClient.Instance.GetSprite(Url, (sprite) => 
                {
                    Sprite = sprite; _downloadInProgress = false;
                });
            }

            /// <summary>
            /// Will return the downloaded image, or download the image, and then return it.
            /// Will return <see langword="null"/> if the download failed.
            /// </summary>
            /// <param name="result">Method to do when image is downloaded.</param>
            public void GetOrDownloadImage(Action<Sprite> result)
            {
                if (Downloaded) result(Sprite);

                if (_downloadInProgress) return;
                _downloadInProgress = true;
                MonetizrClient.Instance.GetSprite(Url, (sprite) =>
                {
                    Sprite = sprite; _downloadInProgress = false;
                    result(sprite);
                });
            }
        }

        /// <summary>
        /// This describes a product variant for the selected variant options <see cref="SelectedOptions"/>.
        /// A variant has it's own price, main image, description and title.
        /// Every product is guaranteed to have at least one variant.
        /// </summary>
        public class Variant
        {
            public string ID;
            public string Title;
            public string Description;
            public string VariantTitle;
            public Dictionary<string, string> SelectedOptions;
            public Price Price;
            public DownloadableImage Image;

            public Variant()
            {
                SelectedOptions = new Dictionary<string, string>();
                Price = new Price();
                Image = new DownloadableImage();
            }

            public override string ToString()
            {
                string output = "";
                var l = SelectedOptions.ToList();
                for (int i = 0; i < l.Count; i++)
                {
                    output += l[i].Value;
                    if (i < l.Count - 1) output += " + ";
                }

                return output;
            }
        }

        public string Tag;
        public string ID;
        public string Title;
        public string Description;
        public string ButtonText;
        public bool AvailableForSale;
        public bool Claimable;

        private string _onlineStoreUrl;
        public List<Option> Options;
        public List<DownloadableImage> Images;
        public List<Variant> Variants;

        //Bad idea - Dto should only be kept as an intermediate step from Json to C#
        //public Dto.ProductByHandle Dto;

        public Product()
        {
            Options = new List<Option>();
            Images = new List<DownloadableImage>();
            Variants = new List<Variant>();
        }

        /// <summary>
        /// Creates a <see cref="Product"/> from data transfer objects used to convert JSON from API 
        /// into C# objects. Developers outside of Monetizr shouldn't be using this for custom functionality, 
        /// as the DTO structure is confusing.
        /// </summary>
        /// <param name="src">The DTO acquired from an API request</param>
        /// <param name="tag">The tag for this product - required, as the DTO does not contain it.</param>
        /// <returns>A perfectly crafted <see cref="Product"/>.</returns>
        public static Product CreateFromDto(Dto.Data src, string tag)
        {
            var p = new Product();
            var pbh = src.productByHandle;

            p.Tag = tag;
            p.ID = pbh.id;
            p.Title = pbh.title;
            p.Description = CleanDescriptionIos(pbh.description_ios);
            p.ButtonText = pbh.button_title;
            p.AvailableForSale = pbh.availableForSale;
            p._onlineStoreUrl = pbh.onlineStoreUrl;
            p.Claimable = pbh.claimable;

            foreach(var o in pbh.options)
            {
                Option newO = new Option();
                newO.Name = o.name;
                newO.Options = o.values;

                p.Options.Add(newO);
            }

            var ie = pbh.images.edges;
            foreach(var i in ie)
            {
                //We should skip gifs because Unity does not display them properly
                //TODO: But what do if only gifs, then very break
                if (i.node.transformedSrc.Contains(".gif")) continue;
                
                p.Images.Add(new DownloadableImage(i.node.transformedSrc));
            }

            var ve = pbh.variants.edges;
            foreach(var v in ve)
            {
                var n = v.node;
                var newV = new Variant();

                Dictionary<string, string> variantOptions = new Dictionary<string, string>();
                foreach (var vo in n.selectedOptions)
                {
                    variantOptions.Add(vo.name, vo.value);
                }

                newV.SelectedOptions = variantOptions;
                newV.ID = n.id;
                newV.VariantTitle = n.title;
                newV.Price.AmountString = n.priceV2.amount;
                newV.Price.CurrencyCode = n.priceV2.currency;
                newV.Price.CurrencySymbol = n.priceV2.currencyCode;

                //Interestingly, even though in JSON compareAtPriceV2 is null, the object exists in C#
                //but the variables inside are null.
                if (n.compareAtPriceV2.amount != null)
                {
                    newV.Price.OriginalAmountString = n.compareAtPriceV2.amount;
                }

                newV.Title = n.product.title;
                newV.Description = CleanDescriptionIos(n.product.description_ios);
                
                if (n.image.transformedSrc.Contains(".gif"))
                {
                    //If a variant hero image is a gif, let's pretend it's the hero image instead
                    //Don't rely on Dto provided hero image because it can be a gif too
                    n.image.transformedSrc = p.Images[0].Url;
                }
                
                newV.Image = new DownloadableImage(n.image.transformedSrc);

                p.Variants.Add(newV);
            }

            return p;
        }

        /// <summary>
        /// Call <see cref="DownloadableImage.DownloadImage"/> on all images on this <see cref="Product"/>.
        /// Excludes variant images.
        /// </summary>
        public void DownloadAllImages()
        {
            foreach(var i in Images)
            {
                if(!i.Downloaded)
                    i.DownloadImage();
            }
        }

        /// <summary>
        /// Get the main image for this <see cref="Product"/>.
        /// </summary>
        /// <returns>The main image</returns>
        public Sprite GetMainImage()
        {
            if (Images.Count == 0) return null;
            return Images[0].Sprite;
        }

        /// <summary>
        /// Gets all images this <see cref="Product"/> contains, excluding variant images.
        /// </summary>
        /// <returns>An array of sprites</returns>
        public Sprite[] GetAllImages()
        {
            List<Sprite> allSprites = new List<Sprite>();
            foreach(var i in Images)
            {
                allSprites.Add(i.Sprite);
            }

            return allSprites.ToArray();
        }

        /// <summary>
        /// Returns <see langword="true"/> if all images, excluding variant images, have been downloaded.
        /// </summary>
        public bool AllImagesDownloaded
        {
            get
            {
                foreach(var i in Images)
                {
                    if (i.Downloaded == false) return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Gets the product <see cref="Variant"/> for given combination of options. 
        /// Returns null if such variant does not exist.
        /// </summary>
        /// <param name="options">key = variant name, value = variant value</param>
        /// <returns>A <see cref="Variant"/> or null, if variant doesn't exist.</returns>
        public Variant GetVariant(Dictionary<string, string> options)
        {
            foreach(var v in Variants)
            {
                bool valid = true;

                if (v.SelectedOptions.Keys.Count != options.Keys.Count)
                    continue;

                foreach(var k in v.SelectedOptions.Keys)
                {
                    if (v.SelectedOptions[k] != options[k])
                    {
                        valid = false;
                        break;
                    }
                }

                if (valid)
                {
                    return v;
                }
            }

            return null; //Could not find a variant with said options
        }

        /// <summary>
        /// Gets the first variant this product contains.
        /// </summary>
        /// <returns>The first variant</returns>
        public Variant GetDefaultVariant()
        {
            //Monetizr API always returns at least one variant.
            return Variants[0];
        }

        /// <summary>
        /// Gets all product variants <see cref="Variant"/> for given combination of options. 
        /// Returns null if there aren't any variants that match the critery.
        /// </summary>
        /// <param name="options">key = variant name, value = variant value</param>
        /// <returns><see cref="Variant"/> array or null, if variants don't exist.</returns>
        public Variant[] GetAllVariantsForOptions(Dictionary<string, string> options)
        {
            List<Variant> matches = new List<Variant>();
            foreach(var v in Variants)
            {
                bool valid = true;

                foreach(var k in options.Keys)
                {
                    if (v.SelectedOptions[k] != options[k])
                    {
                        valid = false;
                        break;
                    }
                }

                if (valid)
                {
                    matches.Add(v);
                }
            }

            return matches.Count > 0 ? matches.ToArray() : null;
        }

        public string GetFormattedPriceRangeForOptions(Dictionary<string, string> options)
        {
            var vars = GetAllVariantsForOptions(options);
            return GetFormattedPriceRangeForVariants(vars);
        }

        public static string GetFormattedPriceRangeForVariants(Variant[] vars)
        {
            if (vars == null) return "Unavailable";

            if (vars.Length == 1) return vars[0].Price.FormattedPrice;
            
            string minPriceText = "";
            decimal minPriceDec = Decimal.MaxValue;
            string maxPriceText = "";
            decimal maxPriceDec = Decimal.MinValue;

            foreach (var v in vars)
            {
                if (v.Price.Amount > maxPriceDec)
                {
                    maxPriceDec = v.Price.Amount;
                    maxPriceText = v.Price.FormattedPrice;
                }
                if (v.Price.Amount < minPriceDec)
                {
                    minPriceDec = v.Price.Amount;
                    minPriceText = v.Price.FormattedPrice;
                }
            }

            if (minPriceText == maxPriceText) return minPriceText;
            return minPriceText + " - " + maxPriceText;
        }

        /// <summary>
        /// Gets the checkout URL for a given <see cref="Variant"/> this <see cref="Product"/> has.
        /// Returns a fallback store URL if obtaining URL failed.
        /// </summary>
        /// <param name="variant">The variant for which to get an URL</param>
        /// <param name="url">Method to do when an URL is obtained.</param>
        public void GetCheckoutUrl(Variant variant, Action<string> url)
        {
            var request = new Dto.VariantStoreObject();
            request.quantity = 1;
            request.product_handle = Tag;
            request.variantId = variant.ID;

            MonetizrClient.Instance.GetCheckoutURL(request, (u) =>
            {
                if (!string.IsNullOrEmpty(u))
                    url(u);
                else
                    url(_onlineStoreUrl);
            });
        }

        public void CreateCheckout(Variant variant, Dto.ShippingAddress address, Action<Checkout> checkout)
        {
            var request = new Dto.CheckoutProductPostData();
            request.quantity = 1;
            request.product_handle = Tag;
            request.variantId = variant.ID;
            request.language = MonetizrClient.Instance.Language;
            request.shippingAddress = address;
            
            MonetizrClient.Instance.PostObjectWithResponse<Dto.CheckoutProductResponse>
                ("products/checkout", request, response =>
            {
                if (response == null) checkout(null);
                var c = Checkout.CreateFromDto(response, address, variant);
                c.SetProduct(this);
                checkout(c);
            });
        }
    }
}

