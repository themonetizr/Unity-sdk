using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Monetizr.UI
{
    public class AlternateVariantsDropdown : MonoBehaviour
    {
        public VariantsDropdown MainDropdown;

        public Text OptionText;
        public Text OptionNameText;

        public void SelectValue()
        {
            MainDropdown.SelectValue();
        }
    }
}

