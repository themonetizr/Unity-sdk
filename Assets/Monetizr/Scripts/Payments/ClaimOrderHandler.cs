using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using Monetizr.Dto;

namespace Monetizr.Payments
{
	public class ClaimOrderHandler : IPaymentHandler
	{
		private Payment _p;

		public void GetResponse(PaymentStatusResponse response)
		{
			throw new System.NotImplementedException();
		}

		public bool IsPolling()
		{
			throw new System.NotImplementedException();
		}

		public void CancelPayment()
		{
			throw new System.NotImplementedException();
		}
		
		public ClaimOrderHandler(Payment payment)
		{
			_p = payment;
		}

		public void Process()
		{
			if (_p == null) return;
			var postData = new ClaimOrderPostData
			{
				checkoutId = _p.Checkout.Id,
				player_id = MonetizrClient.Instance.PlayerId,
				in_game_currency_amount = _p.Checkout.Variant.Price.Amount.ToString()
			};
			
			MonetizrClient.Instance.PostObjectWithResponse<ClaimOrderResponse>
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
					_p.Finish(Payment.PaymentResult.Successful);
					return;
				}
				
				_p.Finish(Payment.PaymentResult.FailedPayment);
			});
		}
	}
}

