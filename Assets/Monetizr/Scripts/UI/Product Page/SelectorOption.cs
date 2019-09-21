using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Monetizr.UI
{
    public class SelectorOption : MonoBehaviour
    {
        public Text OptionNameText;
        public CanvasGroup emphasisLines;
        public SelectionManager SelectionManager;
        public bool IsActive { get; set; }

        public void SetSelected()
        {
            if (!IsActive)
                return;

            SelectionManager.SelectedOption = this;
        }

        public void SetEmphasisLines(bool active)
        {
            emphasisLines.alpha = active ? 1 : 0;
        }

        void Start()
        {
            IsActive = true;
        }
    }
}