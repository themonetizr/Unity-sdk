﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Monetizr
{
    public class VariantsDropdown : MonoBehaviour
    {
        public List<string> Options;
        public string OptionName;
        private List<VariantsDropdown> _allDropdowns;
        public Text OptionText;
        public Text OptionNameText;
        public string SelectedOption;
        public SelectionManager SelectionManager;

        public void Init(List<string> options, string optionName, List<VariantsDropdown> allDropdowns)
        {
            Options = options;
            OptionName = optionName;
            _allDropdowns = allDropdowns;
            SelectedOption = options.FirstOrDefault();
            OptionText.text = SelectedOption;
            OptionNameText.text = optionName.ToUpper();

        }

        public void SelectValue()
        {
            SelectionManager.ShowSelection();
            SelectionManager.InitOptions(Options, OptionName, this, _allDropdowns);
        }
    }
}