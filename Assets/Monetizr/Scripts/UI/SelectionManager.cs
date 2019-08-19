using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Monetizr
{
    public class SelectionManager : MonoBehaviour
    {
        public MonetizrUI ui;
        public List<SelectorOption> Options;
        public SelectorOption SelectedOption
        {
            get { return _selectedOption; }
            set
            {
                _selectedOption = value;
                foreach (var option in Options)
                {
                    if (option.gameObject.GetInstanceID() != _selectedOption.gameObject.GetInstanceID())
                    {
                        option.OptionBaseImage.color = SelectionDeselectedColor;
                        option.OptionNameText.color = SelectionSelectedColor;
                    } 
                }
                _selectedOption.OptionBaseImage.color = SelectionSelectedColor;
                _selectedOption.OptionNameText.color = Color.black;
                var dd = ui.ProductPage.Dropdowns.FirstOrDefault(x => x.OptionName == _optionName);
                dd.OptionText.text = _selectedOption.OptionNameText.text;
                dd.SelectedOption = _selectedOption.OptionNameText.text;
                StartCoroutine(SelectNextEnumerator());
            }
        }

        private IEnumerator SelectNextEnumerator()
        {
            yield return new WaitForSeconds(0.5f);
            NextSelect();
        }

        public Text OptionText;
        public GameObject SelectionPanel;
        public Color SelectionSelectedColor;
        public Color SelectionDeselectedColor;
        //public Color SelectionDisabledColor;
        private SelectorOption _selectedOption;
        string _optionName;
        private VariantsDropdown _currentDropdown;
        private List<VariantsDropdown> _allDropdowns;

        public void InitOptions(List<string> variants, string optionName, VariantsDropdown currentDropdown, List<VariantsDropdown> allDropdowns)
        {
            int i = 0;
            string on = optionName.Replace("Select ", "");
            OptionText.text = "Select " + on + ":";
            _optionName = optionName;
            _currentDropdown = currentDropdown;
            _allDropdowns = allDropdowns;
            foreach (var option in Options)
            {
                option.gameObject.SetActive(false);
            }

            foreach (var variant in variants)
            {
                var option = Options[i];
                option.gameObject.SetActive(true);
                option.OptionNameText.text = variant;
                if (currentDropdown.SelectedOption == variant)
                {
                    option.OptionBaseImage.color = SelectionSelectedColor;
                    option.OptionNameText.color = Color.black;
                }
                else
                {
                    option.OptionBaseImage.color = SelectionDeselectedColor;
                    option.OptionNameText.color = SelectionSelectedColor;
                }
                i++;
            }
        }

        public void ShowSelection()
        {
            SelectionPanel.SetActive(true);
            ui.ProductPage.HideMainLayout();
        }

        public void HideSelection()
        {
            SelectionPanel.SetActive(false);
            ui.ProductPage.ShowMainLayout();
        }

        public void NextSelect()
        {
            int current = _allDropdowns.IndexOf(_currentDropdown);
            var nextDd = _allDropdowns.ElementAtOrDefault(current + 1);
            if (!nextDd || nextDd == null)
            {
                //Never shall ever anyone delete this line to preserve its original glory
                //transform.parent.transform.parent.gameObject.SetActive(false);
                HideSelection();
                return;
            }

            InitOptions(nextDd.Options, nextDd.OptionName, nextDd, _allDropdowns);
        }
    }
}