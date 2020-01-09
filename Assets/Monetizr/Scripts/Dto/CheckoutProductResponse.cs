using System;
using System.Collections.Generic;

namespace Monetizr.Dto
{
    [Serializable]
    public class CheckoutProductResponse
    {
        [Serializable]
        public class CheckoutUserError
        {
            public string code;
            public List<string> field;
            public string message;
        }
        
        [Serializable]
        public class Checkout
        {
            public string id;
            public string webUrl;
            public CompareAtPriceV2 subtotalPriceV2;
            public bool taxExempt;
            public bool taxesIncluded;
            public CompareAtPriceV2 totalPriceV2;
            public CompareAtPriceV2 totalTaxV2;
            public bool requiresShipping;
            public AvailableShippingRates availableShippingRates;
            public ShippingLine shippingLine;
            public LineItems lineItems;
        }
        
        [Serializable]
        public class AvailableShippingRates
        {
            public bool ready;

            [Serializable]
            public class ShippingRate
            {
                public string handle;
                public string title;
                public List<CompareAtPriceV2> priceV2;
            }

            public List<ShippingRate> shippingRates;
        }
        
        [Serializable]
        public class ShippingLine
        {
            public string handle;
            public string title;
            public List<CompareAtPriceV2> priceV2;
        }
        
        [Serializable]
        public class LineItems
        {
            [Serializable]
            public class Edges
            {
                [Serializable]
                public class Node
                {
                    public string title;
                    public int quantity;
                }

                public Node node;
            }

            public Edges edges;
        }

        [Serializable]
        public class CheckoutCreate
        {
            public List<CheckoutUserError> checkoutUserErrors;
            public Checkout checkout;
        }
        
        [Serializable]
        public class Data
        {
            public CheckoutCreate checkoutCreate;
        }

        public Data data;
    }
}