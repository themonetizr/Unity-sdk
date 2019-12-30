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
		}

		public override void InitalizeDropdowns(List<VariantsDropdown> mainDd, bool singular)
		{
			descriptionFieldHorizontal.offsetMin = new Vector2(descriptionFieldHorizontal.offsetMin.x,
				singular ? descriptionFieldBottomNoVariant : _descriptionFieldBottom);
		}
	}
}
