using System;
using System.Collections.Generic;
using UnityEngine;

namespace Monetizr.UI
{
	public class ProductPageVertical : ProductPageLayout {
		public RectTransform bottomBackground;
		private float _bottomBackgroundHeight;
		public float bottomBackgroundHeightNoVariant = 350;

		private void Start()
		{
			_bottomBackgroundHeight = bottomBackground.sizeDelta.y;
			checkoutWindow.Init();
		}

		public override void InitalizeDropdowns(bool singular)
		{
			bottomBackground.sizeDelta = new Vector2(bottomBackground.sizeDelta.x, 
				singular ? bottomBackgroundHeightNoVariant : _bottomBackgroundHeight);
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
