using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Monetizr
{
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

        public class Option
        {
            public string Name;
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

        public class DownloadableImage
        {
            public string Url;
            public Sprite Sprite;
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

            private bool _downloadInProgress = false;
            public void DownloadImage()
            {
                if (_downloadInProgress) return;
                _downloadInProgress = true;
                MonetizrClient.Instance.StartCoroutine(MonetizrClient.Instance.GetSprite(
                    Url, (sprite) => { Sprite = sprite; _downloadInProgress = false; }));
            }
        }

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
        }

        public string Tag;
        public string ID;
        public string Title;
        public string Description;
        public string ButtonText;
        public bool AvailableForSale;

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
                newV.Title = n.product.title;
                newV.Description = CleanDescriptionIos(n.product.description_ios);
                newV.Image = new DownloadableImage(n.image.transformedSrc);

                p.Variants.Add(newV);
            }

            return p;
        }

        public void DownloadAllImages()
        {
            foreach(var i in Images)
            {
                if(!i.Downloaded)
                    i.DownloadImage();
            }
        }

        public Sprite GetMainImage()
        {
            if (Images.Count == 0) return null;
            return Images[0].Sprite;
        }

        public Sprite[] GetAllImages()
        {
            List<Sprite> allSprites = new List<Sprite>();
            foreach(var i in Images)
            {
                allSprites.Add(i.Sprite);
            }

            return allSprites.ToArray();
        }

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

        public Variant GetVariant(Dictionary<string, string> options)
        {
            foreach(var v in Variants)
            {
                bool valid = true;
                foreach(var k in v.SelectedOptions.Keys)
                {
                    if (v.SelectedOptions[k] != options[k])
                    {
                        Debug.Log("invalid variant because " + v.SelectedOptions[k] + "=/=" + options[k]);
                        valid = false;
                        break;
                    }
                }

                if (valid)
                {
                    Debug.Log("found valid variant!");
                    return v;
                }
            }

            return null; //Could not find a variant with said options
        }

        public Variant GetDefaultVariant()
        {
            //Monetizr API always returns at least one variant.
            return Variants[0];
        }

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
    }
}

