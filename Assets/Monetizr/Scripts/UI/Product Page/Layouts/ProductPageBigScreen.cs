using System;
using UnityEngine;
using UnityEngine.UI;

namespace Monetizr.UI
{
	public class ProductPageBigScreen : ProductPageLayout {
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

		public override void InitalizeDropdowns(bool singular)
		{
			// Also do first selection stuff here
			if (firstSelection != null)
			{
				firstSelection.Select();
				firstSelection.OnSelect(null);
			}
			
			base.InitalizeDropdowns(singular);
		}

		public void UpdateImageButtons(int idx)
		{
			prevImageButton.interactable = idx > 0;
			nextImageButton.interactable = idx < imageViewer.DotCount();
		}
	}
}
