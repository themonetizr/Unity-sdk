using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Monetizr.UI
{
    public class SelectorOption : MonoBehaviour
    {
        public Text OptionNameText;
        public CanvasGroup emphasisLines;
        public Text priceText;
        public SelectionManager SelectionManager;
        public bool isActive;

        public void SetSelected()
        {
            if (!isActive)
                return;

            SelectionManager.SelectedOption = this;
        }

        public void SetEmphasisLines(bool active)
        {
            emphasisLines.alpha = active ? 1 : 0;
        }

        void Start()
        {
            isActive = true;
        }
    }
}