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
        public EmphasisLineAnimator emphasisLines;
        public Text priceText;
        public SelectionManager SelectionManager;
        public bool isActive;

        public void SetSelected()
        {
            if (!isActive)
                return;

            SelectionManager.SelectedOption = this;
        }

        public void SetEmphasisLines(bool active, bool force = false)
        {
            if (!gameObject.activeInHierarchy) return;
            
            if (force)
            {
                emphasisLines.StopEase();
                if (active)
                    emphasisLines.Set(1f, 1f);
                else
                    emphasisLines.Set(1.2f, 0f);
            }
            else
            {
                if(active)
                    emphasisLines.DoEase(0.25f, 1.45f, 1f);
                else
                    emphasisLines.DoEase(0.2f, 0f, 0f);
            }
        }

        void Start()
        {
            isActive = true;
        }
    }
}