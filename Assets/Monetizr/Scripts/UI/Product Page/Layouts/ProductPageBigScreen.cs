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

		private void Start()
		{
			imageViewer.ScrollSnap.onRelease.AddListener(UpdateImageButtons);
			checkoutWindow.Init();
		}

		public override void SetOpened(bool opened)
		{
			base.SetOpened(opened);
			
			closeButton.SetActive(opened);
		}

		public void UpdateImageButtons(int idx)
		{
			bool prevButtonWasActive = Equals(EventSystem.current.currentSelectedGameObject, prevImageButton.gameObject);
			bool nextButtonWasActive = Equals(EventSystem.current.currentSelectedGameObject, nextImageButton.gameObject);
			
			prevImageButton.interactable = idx > 0;
			nextImageButton.interactable = idx < imageViewer.DotCount()-1;

			var firstVariantButton = alternateDropdowns[0].GetComponent<Button>();
			var prevButtonNav = prevImageButton.navigation;
			prevButtonNav.selectOnRight = nextImageButton.IsInteractable() ? nextImageButton : firstVariantButton;
			var nextButtonNav = nextImageButton.navigation;
			nextButtonNav.selectOnLeft = prevImageButton.IsInteractable() ? prevImageButton : null;
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
			
			UpdateImageButtons(0);
		}
	}
}
