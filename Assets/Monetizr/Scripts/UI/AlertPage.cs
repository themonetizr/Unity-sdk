using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Monetizr.UI
{
    public class AlertPage : MonoBehaviour
    {
        public CanvasGroup CanvasGroup;
        public Text TitleText;
        public Text MainText;

        public bool IsOpen()
        {
            return CanvasGroup.alpha >= 0.01f;
        }

        public void ShowAlert(string text, string title = "Something isn't working")
        {
            Utility.UIUtility.ShowCanvasGroup(ref CanvasGroup);
            TitleText.text = title;
            MainText.text = text;
        }

        public void HideAlert()
        {
            Utility.UIUtility.HideCanvasGroup(ref CanvasGroup);
        }
    }
}
