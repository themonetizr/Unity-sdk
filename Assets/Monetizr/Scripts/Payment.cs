namespace Monetizr
{
	public class Payment {
		//public string Id { get; private set; }
		public Checkout Checkout { get; private set; }

		public Payment()
		{
			//Id = null;
			Checkout = null;
		}

		public Payment(Checkout checkout)
		{
			//Id = null;
			Checkout = checkout;
		}
		
		
	}
}
