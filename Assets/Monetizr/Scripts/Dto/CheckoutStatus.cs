using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Monetizr.Dto
{
    public class CheckoutStatus
    {
        public string status;
        public string message;
        public string order_number;
    }

    public class CheckoutStatusRequest
    {
        public string checkoutId;
    }
}

