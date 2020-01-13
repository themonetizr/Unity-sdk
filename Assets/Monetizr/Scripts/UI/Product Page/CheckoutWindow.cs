using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Monetizr.UI
{
	public class CheckoutWindow : MonoBehaviour
	{
		private Checkout _currentCheckout = null;
		private Dto.ShippingAddress _currentAddress = null;
		private Price _currentTotalPrice = null;
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

		public VerticalLayoutGroup confirmationPageLayout;
		public ShippingOptionManager shippingOptionManager;
		public Text confirmProductText;
		public Text confirmVariantText;
		public Text deliveryNameText;
		public Text deliveryAddressText;
		public Text subtotalPriceText;
		public Text taxPriceText;
		public Text shippingPriceText;
		public Text totalPriceText;

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
			_currentCheckout = null;
			if (!RequiredFieldsFilled()) return;
			var address = CreateShippingAddress();
			loadingSpinnerAnimator.SetBool(Opened, true);
			shippingPage.interactable = false;
			pp.product.CreateCheckout(pp.CurrentVariant, address, create =>
			{
				OpenConfirmation(create);
			});
		}

		public void SetDefaultShipping()
		{
			shippingOptionManager.SetFirstEnabled();
		}

		public void UpdatePricing()
		{
			var selected = shippingOptionManager.SelectedOption();
			if (selected != null)
			{
				_currentTotalPrice = new Price();
				_currentTotalPrice.CurrencyCode = selected.Price.CurrencyCode;
				_currentTotalPrice.CurrencySymbol = selected.Price.CurrencySymbol;
				decimal total = selected.Price.Amount;
				total += _currentCheckout.Total.Amount;
				// Only allowing string assignments is a weakness of the
				// Price object, but it's a problem we can live with and doesn't pose
				// as a huge performance issue.
				_currentTotalPrice.AmountString = total.ToString();

				shippingPriceText.text = selected.Price.FormattedPrice;
				totalPriceText.text = _currentTotalPrice.FormattedPrice;
			}
			else
			{
				_currentTotalPrice = null;
			}
		}

		private void ForceUpdateConfirmationLayout()
		{
			// Content Size Fitters are nasty things that never work as you would
			// expect them to if you build your UI automatically :<
			
			Canvas.ForceUpdateCanvases();
			confirmationPageLayout.enabled = false;
			confirmationPageLayout.enabled = true;
		}

		public void OpenConfirmation(Checkout checkout)
		{
			if (checkout == null)
			{
				// TODO: Show error to user
				OpenShipping();
				return;
			}
			
			_currentCheckout = checkout;
			_currentAddress = _currentCheckout.ShippingAddress;
			shippingOptionManager.UpdateOptions(checkout.ShippingOptions);
			ForceUpdateConfirmationLayout();
			confirmProductText.text = checkout.Items.First().Title;
			confirmVariantText.text = "1x " + _currentCheckout.Variant;
			deliveryNameText.text = _currentAddress.firstName + " " + _currentAddress.lastName;
			deliveryAddressText.text = _currentAddress.address1 + '\n'
                                        + (string.IsNullOrEmpty(_currentAddress.address2)
                                            ? ""
                                            : (_currentAddress.address2 + '\n'))
                                        + _currentAddress.city +
                                        (string.IsNullOrEmpty(_currentAddress.province)
                                            ? ""
                                            : (", " + _currentAddress.province)) + '\n'
                                        + _currentAddress.zip + '\n'
                                        + ISO3166.FromAlpha2(_currentAddress.country).Name;

			subtotalPriceText.text = _currentCheckout.Subtotal.FormattedPrice;
			taxPriceText.text = _currentCheckout.Tax.FormattedPrice;
			shippingPriceText.text = "Not set!";
			totalPriceText.text = "Not set!";
			SetDefaultShipping();
			loadingSpinnerAnimator.SetBool(Opened, false);
			SetPageState(shippingPage, false);
			SetPageState(confirmationPage, true);
			// TODO
		}

		public void ConfirmCheckout()
		{
			loadingSpinnerAnimator.SetBool(Opened, true);
			confirmationPage.interactable = false;
			var payment = new Payment(_currentCheckout, this);
			// Send new payment to gamedev implemented subscribers
			if(MonetizrClient.Instance.MonetizrPaymentStarted != null)
				MonetizrClient.Instance.MonetizrPaymentStarted(payment);
			else
			{
				Debug.LogError("No subscribers for MonetizrClient.Instance.MonetizrPaymentStarted");
				FinishCheckout(Payment.PaymentResult.NoSubscribers);
			}
		}

		public void FinishCheckout(Payment.PaymentResult result)
		{
			loadingSpinnerAnimator.SetBool(Opened, false);
			SetPageState(shippingPage, false);
			SetPageState(confirmationPage, false);
			//TODO: Page for final message
			var message = "";
			switch (result)
			{
				case Payment.PaymentResult.Successful:
					message = "Order successful! Thank you for your order!";
					break;
				case Payment.PaymentResult.FailedPayment:
					message = "An error occurred while processing the payment.";
					break;
				case Payment.PaymentResult.FailedReport:
					message = "An internal error occured while purchasing.";
					break;
				case Payment.PaymentResult.NoSubscribers:
					message = "No subscribers for payment broadcast delegate.";
					break;
				default:
					throw new ArgumentOutOfRangeException("result", result, null);
			}
			
			
		}
	}
}
