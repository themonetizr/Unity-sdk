using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Monetizr.UI;
using UnityEngine.Serialization;

namespace Monetizr.Utility
{
	public class DisobeySafeArea : MonoBehaviour
	{
		private RectTransform _rect;
		
		public SafeArea safeArea;

		public bool up;
		public bool right;
		public bool left;
		public bool down;

		private Vector2 _initOffsetMin;
		private Vector2 _initOffsetMax;

		public ImageViewer viewerToRefresh;
		private void Start()
		{
			_rect = GetComponent<RectTransform>();
			if (_rect == null) return;
			
			_initOffsetMin = _rect.offsetMin;
			_initOffsetMax = _rect.offsetMax;

			if (safeArea == null)
			{
				this.enabled = false;
				Debug.Log("Give me access to the SafeArea, mate.");
				return;
			}
			
			safeArea.SafeAreaChanged += SafeAreaChanged;

			SafeAreaChanged(Utility.UIUtility.RectFromScreenTo720p(safeArea.GetCurrentSafeArea()));
		}

		private void OnDestroy()
		{
			if (safeArea.SafeAreaChanged != null)
				safeArea.SafeAreaChanged -= SafeAreaChanged;
		}

		private void SafeAreaChanged(Rect area)
		{
			float offsetUp = _initOffsetMax.y,
				offsetRight = _initOffsetMax.x,
				offsetLeft = _initOffsetMin.x,
				offsetDown = _initOffsetMin.y;

			if(right)
				offsetRight = area.width - 720f + area.x;

			if (down)
				offsetDown = -area.y;

			if (left)
				offsetLeft = -area.x;

			if (up)
				offsetUp = area.height - 1280f + area.y;

			_rect.offsetMax = new Vector2(-offsetRight, -offsetUp);
			_rect.offsetMin = new Vector2(offsetLeft, offsetDown);
			
			if(viewerToRefresh != null)
				viewerToRefresh.UpdateCellSize();
		}
	}	
}

