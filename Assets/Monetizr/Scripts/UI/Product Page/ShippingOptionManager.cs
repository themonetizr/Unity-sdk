using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Monetizr.UI
{
	public class ShippingOptionManager : MonoBehaviour
	{
		private List<ShippingOption> _currentOptions = new List<ShippingOption>();
		public GameObject template;
		public Transform optionList;

		public void UpdateOptions(List<Checkout.ShippingRate> rates)
		{
			_currentOptions.ForEach(x => Destroy(x.gameObject));
			_currentOptions.Clear();
			
			rates.ForEach(x =>
			{
				var newGo = Instantiate(template, optionList, true);
				var newOption = newGo.GetComponent<ShippingOption>();
				newOption.CreateFromShippingRate(x);
				newGo.SetActive(true);
				newOption.toggle.isOn = false;
				_currentOptions.Add(newOption);
			});
		}

		public void SetFirstEnabled()
		{
			try
			{
				_currentOptions.First().toggle.isOn = true;
			}
			catch
			{
				Debug.LogWarning("There seems to be no shipping options created - this should not happen.");
			}
		}
		
		public Checkout.ShippingRate SelectedOption()
		{
			var selectedOption = _currentOptions.Find(x => x.toggle.isOn);
			return selectedOption == null ? null : selectedOption.ShippingRate;
		}
	}
}
