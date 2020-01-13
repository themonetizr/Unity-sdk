using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Monetizr.UI
{
	public class CheckoutWindow : MonoBehaviour
	{
		public ProductPageScript pp;
		public Animator animator;
		public Animator loadingSpinnerAnimator;

		public CanvasGroup shippingPage;
		public CanvasGroup confirmationPage;
		
		public InputField firstNameField;
		public InputField lastNameField;
		public InputField emailField;
		public InputField address1Field;
		public InputField address2Field;
		public InputField cityField;
		public InputField provinceField;
		public InputField zipField;
		public Dropdown countryDropdown;
		private static readonly int Opened = Animator.StringToHash("Opened");

		public void Init()
		{
			// Initialize shipping country dropdown
			countryDropdown.options.Clear();
			ISO3166.Collection.ToList().ForEach(x =>
			{
				var option = new Dropdown.OptionData {text = x.Name};
				countryDropdown.options.Add(option);
			});

			countryDropdown.value = countryDropdown.options
				.FindIndex(x => x.text == "United States of America");
		}

		private bool RequiredFieldsFilled()
		{
			if (string.IsNullOrEmpty(address1Field.text))
				return false;
			if (string.IsNullOrEmpty(firstNameField.text))
				return false;
			if (string.IsNullOrEmpty(lastNameField.text))
				return false;
			if (string.IsNullOrEmpty(cityField.text))
				return false;
			//if (string.IsNullOrEmpty(provinceField.text))
			//	return false;
			if (string.IsNullOrEmpty(zipField.text))
				return false;
			if (string.IsNullOrEmpty(emailField.text))
				return false;
			return true;
		}
		
		public Dto.ShippingAddress CreateShippingAddress()
		{
			return new Dto.ShippingAddress
			{
				address1 = address1Field.text,
				address2 = address2Field.text,
				city = cityField.text,
				country = ISO3166.FromName(countryDropdown.options[countryDropdown.value].text).Alpha2,
				firstName = firstNameField.text,
				lastName = lastNameField.text,
				province = provinceField.text,
				zip = zipField.text
			};
		}

		private void SetPageState(CanvasGroup page, bool state)
		{
			page.alpha = state ? 1 : 0;
			page.interactable = state;
			page.blocksRaycasts = state;
		}

		public void CloseWindow()
		{
			animator.SetBool(Opened, false);
		}
		
		public void OpenShipping()
		{
			loadingSpinnerAnimator.SetBool(Opened, false);
			animator.SetBool(Opened, true);
			pp.ui.SelectWhenInteractable(firstNameField);
			SetPageState(shippingPage, true);
			SetPageState(confirmationPage, false);
		}

		public void ConfirmShipping()
		{
			if (!RequiredFieldsFilled()) return;
			var address = CreateShippingAddress();
			loadingSpinnerAnimator.SetBool(Opened, true);
			shippingPage.interactable = false;
			pp.product.CreateCheckout(pp.CurrentVariant, address, create =>
			{
				OpenConfirmation(create);
			});
		}

		public void OpenConfirmation(Checkout checkout)
		{
			if (checkout == null)
			{
				OpenShipping();
				return;
			}
			loadingSpinnerAnimator.SetBool(Opened, false);
			SetPageState(shippingPage, false);
			SetPageState(confirmationPage, true);
			// TODO
		}

		public void ConfirmCheckout()
		{
			// TODO
		}
	}
}
