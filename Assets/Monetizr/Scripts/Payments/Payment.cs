using Monetizr.UI;
using UnityEngine;

namespace Monetizr.Payments
{
	public class Payment {
		//public string Id { get; private set; }
		public Checkout Checkout { get; private set; }
		private CheckoutWindow _caller;

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

		internal void Initiate()
		{
			if (Checkout.Product.Claimable)
			{
				var handler = new ClaimOrderHandler(this);
				handler.Process();
			}
			else
			{
				Finish(PaymentResult.Unimplemented);
			}
		}

		public enum PaymentResult
		{
			Successful,
			FailedPayment,
			Unimplemented
		}
	}
}
