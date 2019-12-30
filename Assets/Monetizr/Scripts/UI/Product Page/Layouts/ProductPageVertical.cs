﻿using System;
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
		}

		public override void InitalizeDropdowns(List<VariantsDropdown> mainDd, bool singular)
		{
			bottomBackground.sizeDelta = new Vector2(bottomBackground.sizeDelta.x, 
				singular ? bottomBackgroundHeightNoVariant : _bottomBackgroundHeight);
			base.InitalizeDropdowns(mainDd, singular);
		}
	}
}
