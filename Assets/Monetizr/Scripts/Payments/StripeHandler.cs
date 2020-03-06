using Monetizr.Dto;

namespace Monetizr.Payments
{
	public class StripeHandler
	{
		private Payment _p;
		
		public StripeHandler(Payment payment)
		{
			_p = payment;
		}
		
		public void Process()
		{
			if (_p == null) return;
			var postData = new PaymentPostData
			{
				product_handle = _p.Checkout.Product.Tag,
				checkoutId = _p.Checkout.Id,
				test = true, //TODO: REMOVE THIS FLAG
				type = "stripe"
			};

			MonetizrClient.Instance.PostObjectWithResponse<PaymentResponse>
			("products/claimorder", postData, resp =>
			{
				if (resp == null)
				{
					_p.Finish(Payment.PaymentResult.FailedPayment, "Internal error occured, you have not been charged.");
					return;
				}
				if (resp.status.Contains("error"))
				{
					_p.Finish(Payment.PaymentResult.FailedPayment, resp.message);
					return;
				}
				
				if (resp.status.Contains("success"))
				{
					
				}
			});
		}
	}
}
