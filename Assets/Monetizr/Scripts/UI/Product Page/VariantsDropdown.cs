using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Monetizr.UI
{
    public class VariantsDropdown : MonoBehaviour
    {
        public List<string> Options;
        public string OptionName;
        private List<VariantsDropdown> _allDropdowns;
        public Text OptionText;
        public Text OptionNameText;
        private string _selectedOption;
        public string SelectedOption
        {
            get { return _selectedOption; }
            set
            {
                _selectedOption = value;
                if (Alternate != null) Alternate.OptionText.text = _selectedOption;
            }
        }
        public SelectionManager SelectionManager;

        public AlternateVariantsDropdown Alternate;

        public void Init(List<string> options, string optionName, List<VariantsDropdown> allDropdowns)
        {
            Options = options;
            OptionName = optionName;
            _allDropdowns = allDropdowns;
            SelectedOption = options.FirstOrDefault();
            OptionText.text = SelectedOption;
            OptionNameText.text = optionName.ToUpper().Replace(' ', '\n');
            if (Alternate != null) Alternate.OptionNameText.text = OptionNameText.text;
        }

        public void SelectValue()
        {
            SelectionManager.ShowSelection();
            SelectionManager.InitOptions(Options, OptionName, this, _allDropdowns);
        }
    }
}