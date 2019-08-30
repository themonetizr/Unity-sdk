using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Monetizr
{
    public class SelectorOption : MonoBehaviour
    {
        public Text OptionNameText;
        public Image OptionBaseImage;
        public SelectionManager SelectionManager;
        public bool IsActive { get; set; }

        public void SetSelected()
        {
            if (!IsActive)
                return;

            SelectionManager.SelectedOption = this;
        }

        void Start()
        {
            IsActive = true;
        }
    }
}