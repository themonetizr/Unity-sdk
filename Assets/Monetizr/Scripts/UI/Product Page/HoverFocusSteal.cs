using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Monetizr.UI
{
	[RequireComponent(typeof(EventTrigger))]
	public class HoverFocusSteal : MonoBehaviour
	{
		private EventTrigger _et;
		private Selectable _s;
		private void Start()
		{
			_et = GetComponent<EventTrigger>();
			_s = GetComponent<Selectable>();

			var enterEvent = new EventTrigger.Entry();
			enterEvent.eventID = EventTriggerType.PointerEnter;
			enterEvent.callback.AddListener((data) =>
			{
				if(_s.IsInteractable())
					EventSystem.current.SetSelectedGameObject(gameObject);
			});
			
			var exitEvent = new EventTrigger.Entry();
			exitEvent.eventID = EventTriggerType.PointerExit;
			exitEvent.callback.AddListener((data) =>
			{
				if(_s.IsInteractable())
					EventSystem.current.SetSelectedGameObject(null);
			});
			
			_et.triggers.Add(enterEvent);
			_et.triggers.Add(exitEvent);
		}
	}
}
