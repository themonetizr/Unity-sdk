using System;

namespace Monetizr.Dto
{
    [Serializable]
    public class ClaimOrderResponse
    {
        public string status;
        public string message;
    }

    [Serializable]
    public class ClaimOrderPostData
    {
        public string checkoutId;
        public string player_id;
        public string in_game_currency_amount;
    }

    [Serializable]
    public class PaymentPostData
    {
        public string product_handle;
        public string checkoutId;
        public string type;
        public bool test;
    }

    [Serializable]
    public class PaymentResponse
    {
        public string status;
        public string message;
        public string intent;
        public string web_url;
    }

    [Serializable]
    public class PaymentStatusPostData
    {
        public string checkoutId;
    }

    [Serializable]
    public class PaymentStatusResponse
    {
        public string status;
        public string message;
        public string payment_status;
        public bool paid;
    }
}