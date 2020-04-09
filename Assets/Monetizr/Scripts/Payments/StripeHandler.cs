using System;
using Monetizr.Dto;
using UnityEngine;

namespace Monetizr.Payments
{
	public class StripeHandler : IPaymentHandler
	{
		private Payment _p;
		private bool _cancelling;
		private bool _polling;
		private PaymentStatusResponse _lastResponse;
		private float _startTime;

		public bool IsPolling()
		{
			return _polling;
		}

		public void CancelPayment()
		{
			_polling = false;
			_cancelling = true;
			GetResponse(_lastResponse);
		}
		
		public StripeHandler(Payment payment)
		{
			_p = payment;
		}

		public void GetResponse(PaymentStatusResponse response)
		{
			if (response.payment_status.Equals("succeeded") && response.paid)
			{
				_p.Finish(Payment.PaymentResult.Successful);
				_polling = false;
			}
			else if (response.payment_status.Equals("processing"))
			{
				if (_cancelling)
				{
					_polling = false;
					_p.Finish(Payment.PaymentResult.FailedPayment, "Payment cancelled");
				}
				else
				{
					if (_startTime + 10 < Time.unscaledTime)
					{
						_p.DisplayCancelButton();
					}
				}
			}
			else
			{
				if (_cancelling)
				{
					_polling = false;
					_p.Finish(Payment.PaymentResult.FailedPayment, "Payment cancelled after failure: " + response.payment_status);
				}
				else
				{
					_p.DisplayCancelButton();
					_p.UpdateStatus("Something went wrong during your last attempt. Press the button below if you wish to cancel.");
				}
			}

			_lastResponse = response;
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
			("products/payment", postData, resp =>
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
					_p.WebGLDisplayContinueButton(resp.web_url, () =>
					{
						_p.UpdateStatus("Waiting for payment to be completed in web browser...");
#if !UNITY_WEBGL
						MonetizrClient.Instance.OpenURL(resp.web_url);
#endif
						_polling = true;
						_startTime = Time.unscaledTime;
						MonetizrClient.Instance.PollPaymentStatus(_p, this);
					});
				}
			});
		}
	}
}
