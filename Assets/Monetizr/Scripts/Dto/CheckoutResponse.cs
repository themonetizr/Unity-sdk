using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monetizr.Dto
{
    [Serializable]
    public class Node
    {
        public string title;
        public int quantity;
    }

    [Serializable]
    public class Edge
    {
        public Node node;
    }

    [Serializable]
    public class LineItems
    {
        public List<Edge> edges;
    }

    [Serializable]
    public class Checkout
    {
        public string id;
        public string webUrl;
        public LineItems lineItems;
    }

    [Serializable]
    public class CheckoutCreate
    {
        public List<object> checkoutUserErrors;
        public Checkout checkout;
    }

    [Serializable]
    public class CheckoutData
    {
        public CheckoutCreate checkoutCreate;
    }

    [Serializable]
    public class CheckoutResponse
    {
        public CheckoutData data;
    }
}
