using System;
using System.Collections.Generic;

namespace Assets.Monetizr.Dto
{
    [Serializable]
    public class ImageNode
    {
        public string transformedSrc ;
    }
    [Serializable]
    public class ImageEdge
    {
        public ImageNode node ;
    }
    [Serializable]
    public class Images
    {
        public List<ImageEdge> edges ;
    }
    [Serializable]
    public class Product
    {
        public string title ;
        public string description ;
        public string descriptionHtml ;
    }
    [Serializable]
    public class PriceV2
    {
        public string currencyCode ;
        public string amount ;
    }
    [Serializable]
    public class ImageInfo
    {
        public string transformedSrc ;
    }
    [Serializable]
    public class VariantsNode
    {
        public string id ;
        public Product product ;
        public string title ;
        public PriceV2 priceV2 ;
        public ImageInfo image ;
        public List<SelectedOptions> selectedOptions ;
    }
    [Serializable]
    public class VariantsEdge
    {
        public VariantsNode node ;
    }
    [Serializable]
    public class Variants
    {
        public List<VariantsEdge> edges ;
    }
    [Serializable]
    public class ProductByHandle
    {
        public string id ;
        public string title ;
        public string description ;
        public string descriptionHtml ;
        public bool availableForSale ;
        public string onlineStoreUrl ;
        public Images images ;
        public Variants variants ;
    }
    [Serializable]
    public class SelectedOptions
    {
        public string name ;
        public string value ;
    }
    [Serializable]
    public class Data
    {
        public ProductByHandle productByHandle ;
    }
    [Serializable]
    public class ProductInfo
    {
        public Data data ;
    }
}
