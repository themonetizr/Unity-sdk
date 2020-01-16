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
		public void Finish(PaymentResult result, string customMessage = null)
		{
			_caller.FinishCheckout(result, customMessage);
		}

		public enum PaymentResult
		{
			Successful,
			FailedPayment,
			FailedReport,
			NoSubscribers,
		}
	}
}
