using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Monetizr
{
    public class SelectionManager : MonoBehaviour
    {
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
                        option.Animator.SetTrigger("Deselect");
                }
                _selectedOption.Animator.SetTrigger("Select");
                var dd = ProductPage.Dropdowns.FirstOrDefault(x => x.OptionName == _optionName);
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
        public ProductPageScript ProductPage;
        private SelectorOption _selectedOption;
        string _optionName;
        private VariantsDropdown _currentDropdown;
        private List<VariantsDropdown> _allDropdowns;

        public void InitOptions(List<string> variants, string optionName, VariantsDropdown currentDropdown, List<VariantsDropdown> allDropdowns)
        {
            int i = 0;
            OptionText.text = "Select " + optionName;
            _optionName = optionName;
            _currentDropdown = currentDropdown;
            _allDropdowns = allDropdowns;
            foreach (var option in Options)
            {
                option.DisableImage.gameObject.SetActive(true);
                option.Animator.SetTrigger("Deselect");
            }

            foreach (var variant in variants)
            {
                var option = Options[i];
                option.OptionNameText.text = variant;
                option.DisableImage.gameObject.SetActive(false);
                i++;
            }
        }

        public void NextSelect()
        {
            int current = _allDropdowns.IndexOf(_currentDropdown);
            var nextDd = _allDropdowns.ElementAtOrDefault(current + 1);
            if (!nextDd || nextDd == null)
            {
                transform.parent.transform.parent.gameObject.SetActive(false);
                return;
            }

            InitOptions(nextDd.Options, nextDd.OptionName, nextDd, _allDropdowns);
        }
    }
}