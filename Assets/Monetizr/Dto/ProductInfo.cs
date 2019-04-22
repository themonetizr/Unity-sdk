using System.Collections.Generic;

namespace Assets.Monetizr.Dto
{

    public class ImageNode
    {
        public string transformedSrc { get; set; }
    }

    public class ImageEdge
    {
        public ImageNode node { get; set; }
    }

    public class Images
    {
        public List<ImageEdge> edges { get; set; }
    }

    public class Product
    {
        public string title { get; set; }
        public string description { get; set; }
        public string descriptionHtml { get; set; }
    }

    public class PriceV2
    {
        public string currencyCode { get; set; }
        public string amount { get; set; }
    }

    public class ImageInfo
    {
        public string transformedSrc { get; set; }
    }

    public class VariantsNode
    {
        public string id { get; set; }
        public Product product { get; set; }
        public string title { get; set; }
        public PriceV2 priceV2 { get; set; }
        public ImageInfo image { get; set; }
        public List<SelectedOptions> selectedOptions { get; set; }
    }

    public class VariantsEdge
    {
        public VariantsNode node { get; set; }
    }

    public class Variants
    {
        public List<VariantsEdge> edges { get; set; }
    }

    public class ProductByHandle
    {
        public string id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string descriptionHtml { get; set; }
        public bool availableForSale { get; set; }
        public string onlineStoreUrl { get; set; }
        public Images images { get; set; }
        public Variants variants { get; set; }
    }

    public class SelectedOptions
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    public class Data
    {
        public ProductByHandle productByHandle { get; set; }
    }

    public class ProductInfo
    {
        public Data data { get; set; }
    }
}
