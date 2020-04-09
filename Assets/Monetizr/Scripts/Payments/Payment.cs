using System;
using Monetizr.UI;
using UnityEditor;
using UnityEngine;

namespace Monetizr.Payments
{
	public class Payment {
		//public string Id { get; private set; }
		public Checkout Checkout { get; private set; }
		private CheckoutWindow _caller;
		private IPaymentHandler _handler;

		public Payment(CheckoutWindow caller)
		{
			Checkout = null;
			_caller = caller;
		}

		public Payment(Checkout checkout, CheckoutWindow caller)
		{
			Checkout = checkout;
			_caller = caller;
		}
		
		/// <summary>
		/// <para>Call this when the payment has been processed or has been failed.</para>
		/// <para>This will finish the loading spinner and display the user a result message</para>
		/// </summary>
		/// <param name="result">Result of this payment</param>
		/// <param name="customMessage">If not null, will override the message displayed</param>
		public void Finish(PaymentResult result, string customMessage = null)
		{
			_caller.FinishCheckout(result, customMessage);
		}

		public void UpdateStatus(string message)
		{
			_caller.UpdateLoadingText(message);
		}

		public void WebGLDisplayContinueButton(string url, Action afterContinue)
		{
#if UNITY_WEBGL
			_caller.SetupLoadingContinueButton(url, afterContinue);
			_caller.UpdateLoadingText("Your payment is ready, click the button to proceed.");
#else
			afterContinue();
#endif
		}

		public void DisplayCancelButton()
		{
			_caller.SetupLoadingCancelButton();
		}

		internal void CancelPayment()
		{
			_handler.CancelPayment();
		}

		internal void Initiate()
		{
			if (Checkout.Product.Claimable)
			{
				_handler = new ClaimOrderHandler(this);
				
			}
			else
			{
				_handler = new StripeHandler(this);
			}
			_handler.Process();
		}

		public enum PaymentResult
		{
			Successful,
			FailedPayment,
			Unimplemented
		}
	}
}
