using System;
using System.Collections;
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
		}
	}
}
