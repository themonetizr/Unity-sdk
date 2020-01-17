using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Monetizr.UI
{
	public class DropdownWithInput : MonoBehaviour
	{
		public Dropdown dropdown;
		public GameObject dropdownLabel;
		public GameObject inputGo;
		private InputField _input;

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
			inputGo.SetActive(false);
			dropdownLabel.SetActive(true);
		}

		public void FilterDropdown()
		{
			var items = ShopifyCountries.Collection.ToList();
			var filter = _input.text.ToLower();
			
			//dropdown.options.Clear();
			if (string.IsNullOrEmpty(filter))
			{
				/*items.ForEach(x =>
				{
					var option = new Dropdown.OptionData {text = x.Name};
					dropdown.options.Add(option);
				});*/
			}
			else
			{
				/*var filtered = items.Where(x => x.Name.ToLower().Contains(filter)).ToList();
				filtered.Sort((i1, i2) => String.Compare(
					i1.Name, i1.Name.IndexOf(filter, StringComparison.InvariantCultureIgnoreCase),
					i2.Name, i2.Name.IndexOf(filter, StringComparison.InvariantCultureIgnoreCase),
					20, StringComparison.InvariantCultureIgnoreCase));
				foreach(var x in filtered)
				{
					var option = new Dropdown.OptionData {text = x.Name};
					dropdown.options.Add(option);
				}*/
				var hits = items.Where(x => x.Name.ToLower().Contains(filter)).ToList();
				hits = hits.OrderBy(x => x.Name.IndexOf(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
				/*hits.Sort((i2, i1) => String.Compare(
					i1.Name.ToLower(), i1.Name.ToLower().IndexOf(filter, StringComparison.InvariantCulture),
					i2.Name.ToLower(), i2.Name.ToLower().IndexOf(filter, StringComparison.InvariantCulture),
					20, StringComparison.InvariantCulture));*/

				//var lat = "Latvia";
				//Debug.Log(lat.ToLower().IndexOf(filter, StringComparison.InvariantCulture));

				dropdown.value = dropdown.options.FindIndex(x => x.text == hits.First().Name);

				string dbg = "";
				hits.ForEach(x => { dbg +=  x.Name + " | "; });
				Debug.Log(dbg);
			}
			
			//dropdown.Hide();
			dropdown.RefreshShownValue();
			//dropdown.Show();
		}

		public void ConfirmFilter()
		{
			/*var last_first = dropdown.options[0];
			var last_first_text = last_first != null ? last_first.text : "United States";
			var items = ShopifyCountries.Collection.ToList();
			dropdown.options.Clear();
			items.ForEach(x =>
			{
				var option = new Dropdown.OptionData {text = x.Name};
				dropdown.options.Add(option);
			});
			
			dropdown.value = dropdown.options
				.FindIndex(x => x.text == last_first_text);
				*/

			DropdownClose();
		}
	}
}
