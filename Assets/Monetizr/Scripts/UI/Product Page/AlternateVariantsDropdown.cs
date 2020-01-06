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

        [Tooltip("Use this field if this dropdown belongs to a big screen layout")]
        public SelectionManagerBigScreen selectionManagerBigScreen;

        public void SelectValue()
        {
            if(selectionManagerBigScreen == null)
                MainDropdown.SelectValue();
            else
            {
                if (selectionManagerBigScreen.IsOpen())
                {
                    selectionManagerBigScreen.HideSelection();
                    return;
                }
                selectionManagerBigScreen.ShowSelection();
                selectionManagerBigScreen.InitOptions(
                    MainDropdown.Options, MainDropdown.OptionName,
                    MainDropdown, MainDropdown.AllDropdowns);
            }
        }
    }
}

