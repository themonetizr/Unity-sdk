using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Monetizr
{
    public class AlertPage : MonoBehaviour
    {
        public CanvasGroup CanvasGroup;
        public Text TitleText;
        public Text MainText;
        
        public void ShowAlert(string text, string title = "Something isn't working")
        {
            Utility.UIUtilityScript.ShowCanvasGroup(ref CanvasGroup);
            TitleText.text = title;
            MainText.text = text;
        }

        public void HideAlert()
        {
            Utility.UIUtilityScript.HideCanvasGroup(ref CanvasGroup);
        }
    }
}
