using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Monetizr.UI {
	public class PressHandler : MonoBehaviour, IPointerDownHandler
	{
        private Button _btn;

        private void Start()
        {
            _btn = GetComponent<Button>();
            if(_btn == null)
            {
                Debug.LogWarning("PressHandler could not find Button component. Disabling!");
                enabled = false;
            }
        }

        [Serializable]
		public class ButtonPressEvent : UnityEvent { } 

		public ButtonPressEvent OnPress = new ButtonPressEvent();

		public void OnPointerDown(PointerEventData eventData)
		{
            if(_btn.IsInteractable())
			    OnPress.Invoke();
		}
	}
}