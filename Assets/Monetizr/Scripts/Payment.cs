using Monetizr.UI;

namespace Monetizr
{
	public class Payment {
		//public string Id { get; private set; }
		public Checkout Checkout { get; private set; }
		private CheckoutWindow _caller;

		public Payment(CheckoutWindow caller)
		{
			//Id = null;
			Checkout = null;
			_caller = caller;
		}

		public Payment(Checkout checkout, CheckoutWindow caller)
		{
			//Id = null;
			Checkout = checkout;
			_caller = caller;
		}

		// TODO: Write summaries - this is game dev facing API
		/// <summary>
		/// If this summary hasn't been written send an angry email to rudolfs@themonetizr.com
		/// </summary>
		public void Successful()
		{
			// TODO: Return information about successful payment to Monetizr
			_caller.FinishCheckout(PaymentResult.Successful);
			// This should scream RED F*IN ALERT IF FAILS because then we would be
			// have charged the user without sending anything
			// TODO: Use MonetizrPaymentForwardingFailed to handle such unfortunate events
		}
		
		// TODO: Write summaries - this is game dev facing API
		/// <summary>
		/// If this summary hasn't been written send an angry email to rudolfs@themonetizr.com
		/// </summary>
		public void Failed(PaymentResult reason)
		{
			_caller.FinishCheckout(reason);
		}

		public enum PaymentResult
		{
			Successful,
			FailedPayment,
			FailedReport,
			NoSubscribers
		}
	}
}
