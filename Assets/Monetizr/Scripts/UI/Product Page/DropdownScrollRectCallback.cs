using UnityEngine;
using UnityEngine.UI;

namespace Monetizr.UI
{
	public class DropdownScrollRectCallback : MonoBehaviour
	{
		public DropdownWithInput ddwi;
		private ScrollRect _scrollRect;
		
		void Start()
		{
			_scrollRect = GetComponent<ScrollRect>();
			ddwi.SetCurrentScrollRect(_scrollRect);
		}
	}
}
