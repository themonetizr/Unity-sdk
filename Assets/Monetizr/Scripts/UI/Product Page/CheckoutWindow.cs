using System;
using System.Collections;
using System.Collections.Generic;
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
		public CanvasGroup resultPage;

		private enum Page
		{
			NoPage,
			ShippingPage,
			ConfirmationPage,
			ResultPage
		}
		
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

		public Text resultPageHeader;
		public Text resultPageText;

		public Animator errorWindowAnimator;
		public VerticalLayoutGroup errorWindowLayout;
		public Text errorWindowText;

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

		private void OpenPage(Page page)
		{
			SetPageState(shippingPage, page == Page.ShippingPage);
			SetPageState(confirmationPage, page == Page.ConfirmationPage);
			SetPageState(resultPage, page == Page.ResultPage);
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
			OpenPage(Page.ShippingPage);
		}

		public void ConfirmShipping()
		{
			_currentCheckout = null;
			if (!RequiredFieldsFilled())
			{
				SetErrorWindowState(true);
				var e = new Checkout.Error("Please fill all required fields", "aaa");
				var l = new List<Checkout.Error> {e};
				WriteErrorWindow(l);
				return;
			}
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

				_currentCheckout.SetShippingLine(selected);
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
				SetErrorWindowState(true);
				var e = new Checkout.Error("Internal server error", "aaa");
				var l = new List<Checkout.Error> {e};
				WriteErrorWindow(l);
				OpenShipping();
				return;
			}

			if (checkout.Errors.Count > 0)
			{
				SetErrorWindowState(true);
				WriteErrorWindow(checkout.Errors);
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
			OpenPage(Page.ConfirmationPage);
			// TODO
		}

		public void ConfirmCheckout()
		{
			// TODO: Test if all required fields are set in Payment object
			
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
			OpenPage(Page.ResultPage);
			//TODO: Page for final message
			var message = "";
			switch (result)
			{
				case Payment.PaymentResult.Successful:
					message = "Thank you for your order!";
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

			resultPageHeader.text = result == Payment.PaymentResult.Successful ? "Awesome!" : "Oops!";
			if (result == Payment.PaymentResult.Successful)
			{
				message += " Your " + _currentCheckout.Items.First().Title + " is on it's way!";
			}
			resultPageText.text = message;
		}

		public void SetErrorWindowState(bool state)
		{
			errorWindowAnimator.SetBool(Opened, state);
		}

		public void WriteErrorWindow(List<Checkout.Error> errors)
		{
			string s = "";
			errors.ForEach(x =>
			{
				s += x.Message;
				s += '\n';
			});
			// Remove the last newline
			s = s.Substring(0, s.Length - 1);
			errorWindowText.text = s;
			Canvas.ForceUpdateCanvases();
			errorWindowLayout.enabled = false;
			errorWindowLayout.enabled = true;
		}
	}
}
