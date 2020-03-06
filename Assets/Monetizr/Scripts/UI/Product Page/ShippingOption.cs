using UnityEngine;
using UnityEngine.UI;

namespace Monetizr.UI
{
	public class ShippingOption : MonoBehaviour
	{
		public Checkout.ShippingRate ShippingRate { get; private set; }
		public Toggle toggle;
		public Text nameText;
		public Text priceText;
		
		public void CreateFromShippingRate(Checkout.ShippingRate shippingRate)
		{
			ShippingRate = shippingRate;

			toggle.isOn = false;
			nameText.text = ShippingRate.Title;
			priceText.text = ShippingRate.Price.FormattedPrice;
		}

	}
}
