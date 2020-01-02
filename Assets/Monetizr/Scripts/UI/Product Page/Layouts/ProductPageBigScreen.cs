using UnityEngine;
using UnityEngine.UI;

namespace Monetizr.UI
{
	public class ProductPageBigScreen : ProductPageLayout {
		public Selectable firstSelection;

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
	}
}
