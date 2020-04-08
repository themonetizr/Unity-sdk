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

		public override void UpdateButtons()
		{
			// Use current index
			UpdateButtons(_lastIdx);
		}
		
		public override void UpdateButtons(int idx)
		{
			bool prevButtonWasActive = Equals(EventSystem.current.currentSelectedGameObject, prevImageButton.gameObject);
			bool nextButtonWasActive = Equals(EventSystem.current.currentSelectedGameObject, nextImageButton.gameObject);
			
			prevImageButton.interactable = idx > 0;
			nextImageButton.interactable = idx < imageViewer.DotCount()-1;

			Button firstVariantButton = null;
			Button lastVariantButton = null;// = alternateDropdowns[0].GetComponent<Button>();
			for (int i = 0; i < alternateDropdowns.Count; i++)
			{
				Button prev = i >= 1 ? alternateDropdowns[i - 1].GetComponent<Button>() : null;
				Button cur = alternateDropdowns[i].GetComponent<Button>();
				Button next = i <= 1 ? alternateDropdowns[i + 1].GetComponent<Button>() : checkoutButton;
				var nav = cur.navigation;
				if (!next.gameObject.activeSelf)
				{
					next = checkoutButton;
					lastVariantButton = cur;
				}
				nav.selectOnLeft = prev;
				nav.selectOnRight = next;
				if (cur.gameObject.activeSelf && i == 0)
				{
					firstVariantButton = cur;
				}

				cur.navigation = nav;
			}

			var checkoutButtonNav = checkoutButton.navigation;
			checkoutButtonNav.selectOnLeft = lastVariantButton;
			checkoutButton.navigation = checkoutButtonNav;
			
			var nextButtonNav = nextImageButton.navigation;
			nextButtonNav.selectOnLeft = prevImageButton.IsInteractable() ? prevImageButton : null;
			
			// Handle various CheckoutWindow cases
			if (checkoutWindow != null)
			{
				switch (checkoutWindow.CurrentPage())
				{
					case CheckoutWindow.Page.NoPage:
						nextButtonNav.selectOnRight = firstVariantButton != null ? firstVariantButton : checkoutButton;
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
					case CheckoutWindow.Page.CheckoutUpdatePage:
						nextButtonNav.selectOnRight = checkoutWindow.checkoutUpdatePageNavSelection;
						_closeNav.selectOnDown = checkoutWindow.checkoutUpdatePageNavSelection;
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
			if (firstVariantButton != null)
			{
				var firstVariantNav = firstVariantButton.navigation;
				if (imageViewer.DotCount() > 1)
					firstVariantNav.selectOnLeft = nextImageButton.interactable ? nextImageButton : prevImageButton;
				else
					firstVariantNav.selectOnLeft = null;
				firstVariantButton.navigation = firstVariantNav;
			}

			prevImageButton.navigation = prevButtonNav;
			nextImageButton.navigation = nextButtonNav;
			
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
			base.OnFinishedLoading();
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
