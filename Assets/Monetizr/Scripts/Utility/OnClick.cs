using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Monetizr.UI
{
	public class OnClick : MonoBehaviour, IPointerClickHandler
	{
		public UnityEvent clickEvent;

		public void OnPointerClick(PointerEventData eventData)
		{
			clickEvent.Invoke();
		}
	}
}
