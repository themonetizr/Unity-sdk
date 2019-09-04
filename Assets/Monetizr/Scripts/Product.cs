using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Monetizr
{
    public class Product
    {
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
            }
        }

        public string ID;
        public string Title;
        public string Description;
        public bool AvailableForSale;

        private string _onlineStoreUrl;
        public List<Option> Options;
        public List<DownloadableImage> Images;
        public List<Variant> Variants;

        public static Product CreateFromDto(Dto.Data src)
        {
            throw new NotImplementedException();
        }

        public void DownloadAllImages()
        {
            throw new NotImplementedException();
        }

        public Sprite GetMainImage()
        {
            throw new NotImplementedException();
        }

        public Sprite[] GetAllImages()
        {
            throw new NotImplementedException();
        }

        public Variant GetVariant(Dictionary<string, string> options)
        {
            throw new NotImplementedException();
        }

        public Variant GetDefaultVariant()
        {
            throw new NotImplementedException();
        }

        public string GetCheckoutUrl(Action<string> url)
        {
            throw new NotImplementedException();
        }
    }
}

