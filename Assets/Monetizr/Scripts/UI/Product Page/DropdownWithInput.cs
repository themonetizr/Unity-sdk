using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Monetizr.UI
{
	public class DropdownWithInput : MonoBehaviour
	{
		public MonetizrUI ui;
		public Dropdown dropdown;
		public RectTransform itemTemplate;
		public GameObject dropdownLabel;
		public GameObject inputGo;
		private InputField _input;
		private ScrollRect _scrolLRect;

		private void Start()
		{
			_input = inputGo.GetComponent<InputField>();
			DropdownClose();
		}

		public void DropdownOpen()
		{
			inputGo.SetActive(true);
			_input.text = "";
			dropdownLabel.SetActive(false);
			EventSystem.current.SetSelectedGameObject(inputGo);
		}

		public void DropdownClose()
		{
			//if (!dropdown.IsInteractable()) return;
			inputGo.SetActive(false);
			dropdownLabel.SetActive(true);
			dropdown.Hide();
		}

		public void SetCurrentScrollRect(ScrollRect scrollRect)
		{
			_scrolLRect = scrollRect;
		}

		public void FilterDropdown()
		{
			var items = ShopifyCountries.Collection.ToList();
			var filter = _input.text.ToLower();
			
			//dropdown.options.Clear();
			if (!string.IsNullOrEmpty(filter))
			{
				var hits = items.Where(x => x.Name.ToLower().Contains(filter)).ToList();
				if (hits.Count > 0)
				{
					hits = hits.OrderBy(x => x.Name.IndexOf(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
					dropdown.value = dropdown.options.FindIndex(x => x.text == hits.First().Name);
				}
			}

			if (_scrolLRect != null)
			{
				float pos = 1f - dropdown.value / ((float) dropdown.options.Count - 1);
				//float pos_curved = (Mathf.Cos(pos * Mathf.PI) + 1f) / 2f;
				_scrolLRect.verticalNormalizedPosition = pos;
			}
			
			dropdown.RefreshShownValue();
		}

		public void ConfirmFilter()
		{
			DropdownClose();
			ui.SelectWhenInteractable(dropdown);
		}

		public void RestoreDropdown()
		{
			inputGo.SetActive(false);
			dropdownLabel.SetActive(true);
		}
	}
}
