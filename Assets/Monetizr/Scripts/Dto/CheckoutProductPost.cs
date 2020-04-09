using System;

namespace Monetizr.Dto
{
    [Serializable]
    public class ShippingAddress
    {
        public string firstName;
        public string lastName;
        public string address1;
        public string address2;
        public string city;
        public string country;
        public string zip;
        public string province;
    }

    [Serializable]
    public class CheckoutProductPostData
    {
        public string product_handle;
        public string variantId;
        public int quantity;
        public string language;
        public ShippingAddress shippingAddress;
    }
    
    [Serializable]
    public class CheckoutUpdatePostData
    {
        public string product_handle;
        public string checkoutId;
        public string email;
        public string shippingRateHandle;
        public ShippingAddress shippingAddress;
        public ShippingAddress billingAddress;
    }
}