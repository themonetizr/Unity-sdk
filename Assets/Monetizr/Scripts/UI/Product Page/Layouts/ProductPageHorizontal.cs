using System.Collections.Generic;
using UnityEngine;

namespace Monetizr.UI
{
	public class ProductPageHorizontal : ProductPageLayout {
		public RectTransform descriptionFieldHorizontal;
		private float _descriptionFieldBottom;
		public float descriptionFieldBottomNoVariant = 230;

		private void Start()
		{
			_descriptionFieldBottom = descriptionFieldHorizontal.offsetMin.y;
			//TODO: checkoutWindow.Init();
		}

		public override void InitalizeDropdowns(bool singular)
		{
			descriptionFieldHorizontal.offsetMin = new Vector2(descriptionFieldHorizontal.offsetMin.x,
				singular ? descriptionFieldBottomNoVariant : _descriptionFieldBottom);
			base.InitalizeDropdowns(singular);
		}

		public override void UpdateButtons()
		{
			// Not required for mobile views
			// TODO: Handle system back button
		}

		public override void UpdateButtons(int idx)
		{
			// Not required for mobile views
			// TODO: Handle system back button
		}
	}
}
