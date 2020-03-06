using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Monetizr.UI
{
	public class ProductPageBigScreen : ProductPageLayout
	{
		public MonetizrUI ui;
		public Selectable firstSelection;

		public GameObject closeButton;
		public Button prevImageButton;
		public Button nextImageButton;
		
		public Button closeButtonButton;
		private Navigation _closeNav;
		private Selectable _closeNavDownDefault;

		private int _lastIdx = 0;

		private void Start()
		{
			imageViewer.ScrollSnap.onRelease.AddListener(UpdateButtons);
			checkoutWindow.Init();

			_closeNav = closeButtonButton.GetComponent<Button>().navigation;
			_closeNavDownDefault = _closeNav.selectOnDown;
		}

		public void StartCheckout()
		{
			if (ui.ProductPage.selectionManagerBigScreen.IsOpen())
			{
				ui.ProductPage.selectionManagerBigScreen.HideSelection(true);
				return;
			}
			checkoutWindow.OpenShipping();
		}

		public override void SetOpened(bool opened)
		{
			base.SetOpened(opened);
			
			closeButton.SetActive(opened);
		}

		public void UpdateButtons()
		{
			// Use current index
			UpdateButtons(_lastIdx);
		}
		
		public void UpdateButtons(int idx)
		{
			bool prevButtonWasActive = Equals(EventSystem.current.currentSelectedGameObject, prevImageButton.gameObject);
			bool nextButtonWasActive = Equals(EventSystem.current.currentSelectedGameObject, nextImageButton.gameObject);
			
			prevImageButton.interactable = idx > 0;
			nextImageButton.interactable = idx < imageViewer.DotCount()-1;

			var firstVariantButton = alternateDropdowns[0].GetComponent<Button>();
			var nextButtonNav = nextImageButton.navigation;
			nextButtonNav.selectOnLeft = prevImageButton.IsInteractable() ? prevImageButton : null;
			
			// Handle various CheckoutWindow cases
			if (checkoutWindow != null)
			{
				switch (checkoutWindow.CurrentPage())
				{
					case CheckoutWindow.Page.NoPage:
						nextButtonNav.selectOnRight = firstVariantButton;
						_closeNav.selectOnDown = _closeNavDownDefault;
						break;
					case CheckoutWindow.Page.ShippingPage:
						nextButtonNav.selectOnRight = checkoutWindow.shippingPageNavSelection;
						_closeNav.selectOnDown = checkoutWindow.shippingPageNavSelection;
						break;
					case CheckoutWindow.Page.ConfirmationPage:
						nextButtonNav.selectOnRight = checkoutWindow.confirmationPageNavSelection;
						_closeNav.selectOnDown = checkoutWindow.confirmationPageNavSelection;
						break;
					case CheckoutWindow.Page.ResultPage:
						nextButtonNav.selectOnRight = checkoutWindow.resultPageNavSelection;
						_closeNav.selectOnDown = checkoutWindow.resultPageNavSelection;
						break;
					case CheckoutWindow.Page.SomePage:
						nextButtonNav.selectOnRight = null;
						_closeNav.selectOnDown = nextImageButton.IsInteractable() ? nextImageButton : prevImageButton;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}

				closeButtonButton.navigation = _closeNav;
			}

			var prevButtonNav = prevImageButton.navigation;
			prevButtonNav.selectOnRight = nextImageButton.IsInteractable() ? nextImageButton : nextButtonNav.selectOnRight;
			var firstVariantNav = firstVariantButton.navigation;
			if (imageViewer.DotCount() > 1)
				firstVariantNav.selectOnLeft = nextImageButton.interactable ? nextImageButton : prevImageButton;
			else
				firstVariantNav.selectOnLeft = null;

			prevImageButton.navigation = prevButtonNav;
			nextImageButton.navigation = nextButtonNav;
			firstVariantButton.navigation = firstVariantNav;
			
			if (!prevImageButton.IsInteractable() && prevButtonWasActive)
			{
				ui.SelectWhenInteractable(nextImageButton);
			}
			
			if (!nextImageButton.IsInteractable() && nextButtonWasActive)
			{
				ui.SelectWhenInteractable(prevImageButton);
			}
			_lastIdx = idx;
		}

		public override void OnFinishedLoading()
		{
			if (firstSelection == null) return;
			ui.SelectWhenInteractable(firstSelection);
			
			Canvas.ForceUpdateCanvases();
			alternateDropdowns.ForEach(x =>
			{
				x.GetComponent<HorizontalLayoutGroup>().enabled = false;
				x.GetComponent<HorizontalLayoutGroup>().enabled = true;
			});
			
			UpdateButtons(0);
		}
	}
}
