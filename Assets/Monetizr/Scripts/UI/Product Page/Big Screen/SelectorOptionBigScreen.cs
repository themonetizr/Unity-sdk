using UnityEngine;
using UnityEngine.UI;

namespace Monetizr.UI
{
	public class SelectorOptionBigScreen : MonoBehaviour {
		public SelectionManagerBigScreen manager;
		public Text optionNameText;
		public Text priceText;

		public void SetSelected()
		{
			manager.SelectedOption = this;
		}
	}
}
