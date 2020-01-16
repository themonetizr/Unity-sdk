using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Monetizr.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Monetizr.UI
{
	public class CheckoutWindow : MonoBehaviour
	{
		public bool Working { get; private set; }

		public bool IsOpen
		{
			get { return animator.GetBool(Opened); }
		}

		private Checkout _currentCheckout = null;
		private Dto.ShippingAddress _currentAddress = null;
		private Price _currentTotalPrice = null;
		public ProductPageScript pp;
		public ProductPageBigScreen layout;
		public Animator animator;
		public Animator loadingSpinnerAnimator;

		public CanvasGroup windowGroup;
		public CanvasGroup shippingPage;
		public CanvasGroup confirmationPage;
		public CanvasGroup resultPage;

		public Selectable shippingPageNavSelection;
		public Selectable confirmationPageNavSelection;
		public Selectable resultPageNavSelection;

		public enum Page
		{
			NoPage,
			ShippingPage,
			ConfirmationPage,
			ResultPage,
			SomePage
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
		public GameObject policyLinks;
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
		public Button errorWindowCloseButton;

		private const string PrivacyPolicyUrl = "https://www.themonetizr.com/privacy-policy";

		public void OpenPrivacyPolicy()
		{
			MonetizrClient.Instance.OpenURL(PrivacyPolicyUrl);
		}
		
		private const string TermsOfServiceUrl = "https://www.themonetizr.com/terms-of-service";

		public void OpenTermsOfService()
		{
			MonetizrClient.Instance.OpenURL(TermsOfServiceUrl);
		}
		
		public CheckoutWindow()
		{
			Working = false;
		}

		public void Init()
		{
			// Initialize shipping country dropdown
			countryDropdown.options.Clear();
			ShopifyCountries.Collection.ToList().ForEach(x =>
			{
				var option = new Dropdown.OptionData {text = x.Name};
				countryDropdown.options.Add(option);
			});

			countryDropdown.value = countryDropdown.options
				.FindIndex(x => x.text == "United States");
			
			policyLinks.SetActive(MonetizrClient.Instance.PolicyLinksEnabled);
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
				country = ShopifyCountries.FromName(countryDropdown.options[countryDropdown.value].text).Alpha2,
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
			layout.UpdateButtons();
			switch (page)
			{
				case Page.NoPage:
					break;
				case Page.ShippingPage:
					pp.ui.SelectWhenInteractable(shippingPageNavSelection);
					break;
				case Page.ConfirmationPage:
					pp.ui.SelectWhenInteractable(confirmationPageNavSelection);
					break;
				case Page.ResultPage:
					pp.ui.SelectWhenInteractable(resultPageNavSelection);
					break;
				case Page.SomePage:
					break;
				default:
					throw new ArgumentOutOfRangeException("page", page, null);
			}
		}

		private void SetLoading(bool state)
		{
			loadingSpinnerAnimator.SetBool(Opened, state);
			layout.UpdateButtons();
		}

		public Page CurrentPage()
		{
			if (!IsOpen)
				return Page.NoPage;
			if (loadingSpinnerAnimator.GetBool(Opened))
				return Page.SomePage;
			if (shippingPage.alpha > 0.01)
				return Page.ShippingPage;
			if (confirmationPage.alpha > 0.01)
				return Page.ConfirmationPage;
			if (resultPage.alpha > 0.01)
				return Page.ResultPage;
			return Page.SomePage; //Not open already checked at first line
		}
		
		private void SetPageState(CanvasGroup page, bool state)
		{
			page.alpha = state ? 1 : 0;
			page.interactable = state;
			page.blocksRaycasts = state;
		}

		public void CloseWindow()
		{
			if (Working) return;
			if (!IsOpen) return;
			animator.SetBool(Opened, false);
			SetErrorWindowState(false);
			pp.ui.SelectWhenInteractable(layout.firstSelection);
			layout.UpdateButtons();
		}
		
		public void OpenShipping()
		{
			SetLoading(false);
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
			SetLoading(true);
			shippingPage.interactable = false;
			Working = true;
			pp.product.CreateCheckout(pp.CurrentVariant, address, create =>
			{
				OpenConfirmation(create);
				Working = false;
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
                                        + ShopifyCountries.FromAlpha2(_currentAddress.country).Name;

			subtotalPriceText.text = _currentCheckout.Subtotal.FormattedPrice;
			taxPriceText.text = _currentCheckout.Tax.FormattedPrice;
			shippingPriceText.text = "Not set!";
			totalPriceText.text = "Not set!";
			SetDefaultShipping();
			SetLoading(false);
			OpenPage(Page.ConfirmationPage);
		}

		public void ConfirmCheckout()
		{
			SetLoading(true);
			confirmationPage.interactable = false;
			var payment = new Payment(_currentCheckout, this);
			Working = true;
			payment.Initiate();
		}

		public void FinishCheckout(Payment.PaymentResult result, string msg = null)
		{
			SetLoading(false);
			OpenPage(Page.ResultPage);
			Working = false;
			var message = msg ?? "";
			if (msg == null)
			{
				switch (result)
				{
					case Payment.PaymentResult.Successful:
						message = "Thank you for your order!";
						break;
					case Payment.PaymentResult.FailedPayment:
						message = "An error occurred while processing the payment.";
						break;
					case Payment.PaymentResult.NoSubscribers:
						message = "No subscribers for payment broadcast delegate.";
						break;
					default:
						throw new ArgumentOutOfRangeException("result", result, null);
				}
			}

			resultPageHeader.text = result == Payment.PaymentResult.Successful ? "Awesome!" : "Oops!";
			if (result == Payment.PaymentResult.Successful && msg == null)
			{
				message += " Your " + _currentCheckout.Items.First().Title + " is on it's way!";
			}
			resultPageText.text = message;
		}

		public void SetErrorWindowState(bool state)
		{
			errorWindowAnimator.SetBool(Opened, state);
			pp.ui.SelectWhenInteractable(errorWindowCloseButton);
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
