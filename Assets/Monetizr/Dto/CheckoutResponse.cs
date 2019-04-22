using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Monetizr.Dto
{
    public class Node
    {
        public string title { get; set; }
        public int quantity { get; set; }
    }

    public class Edge
    {
        public Node node { get; set; }
    }

    public class LineItems
    {
        public List<Edge> edges { get; set; }
    }

    public class Checkout
    {
        public string id { get; set; }
        public string webUrl { get; set; }
        public LineItems lineItems { get; set; }
    }

    public class CheckoutCreate
    {
        public List<object> checkoutUserErrors { get; set; }
        public Checkout checkout { get; set; }
    }

    public class CheckoutData
    {
        public CheckoutCreate checkoutCreate { get; set; }
    }

    public class CheckoutResponse
    {
        public CheckoutData data { get; set; }
    }
}
