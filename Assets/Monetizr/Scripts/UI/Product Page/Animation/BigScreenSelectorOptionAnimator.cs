using System;
using System.Collections;
using UnityEngine;
using UnityEngineInternal;

namespace Monetizr.UI
{
	public class BigScreenSelectorOptionAnimator : MonoBehaviour
	{
		public static float In (float k) {
			return k*k*k;
		}
		
		public static float Out (float k) {
			return 1f + ((k -= 1f)*k*k);
		}

		private bool _animating = false;
		public bool Animating
		{
			get { return _animating; }
		}
		
		private RectTransform _rect;
		private CanvasGroup _cg;

		private void Start()
		{
			_rect = GetComponent<RectTransform>();
			_cg = GetComponent<CanvasGroup>();
			StartCoroutine(_LeaveAnimation(0f));
		}

		public void SetClosed()
		{
			StartCoroutine(_LeaveAnimation(0f));
		}
		
		//Open animation
		public void OpenAnimation(float length = 0.2f)
		{
			if (_animating) return;
			StartCoroutine(_OpenAnimation(length));
		}
		
		private IEnumerator _OpenAnimation(float length)
		{
			_animating = true;
			float t = 0f;
			if (length <= 0.00001f) t = 999f;
			float startTime = Time.unscaledTime;

			var sd = _rect.sizeDelta;
			sd.y = 0f;
			_cg.alpha = 0f;
			_cg.blocksRaycasts = false;
			_rect.sizeDelta = sd;
			
			while (t < 1f)
			{
				sd.y = 80f * Out(t);
				_cg.alpha = (float)Math.Pow(Out(t), 5f);
				_rect.sizeDelta = sd;
				t = (Time.unscaledTime - startTime) / length;
				yield return new WaitForEndOfFrame();
			}

			sd.y = 80f;
			_cg.alpha = 1f;
			_cg.blocksRaycasts = true;
			_rect.sizeDelta = sd;
			_animating = false;
		}
		
		//Select animation
		public void SelectAnimation(float length = 0.2f)
		{
			if (_animating) return;
			StartCoroutine(_SelectAnimation(length));
		}
		
		private IEnumerator _SelectAnimation(float length)
		{
			_animating = true;
			float t = 0f;
			if (length <= 0.00001f) t = 999f;
			float startTime = Time.unscaledTime;

			var sd = _rect.sizeDelta;
			sd.y = 80f;
			_cg.alpha = 1f;
			_rect.sizeDelta = sd;
			
			while (t < 1f)
			{
				sd.y = 80f + 20f * In(t);
				_rect.sizeDelta = sd;
				t = (Time.unscaledTime - startTime) / length;
				yield return new WaitForEndOfFrame();
			}

			sd.y = 100f;
			_rect.sizeDelta = sd;
			_animating = false;
		}
		
		//Leave animation
		public void LeaveAnimation(float length = 0.2f)
		{
			if (_animating) return;
			StartCoroutine(_LeaveAnimation(length));
		}
		
		private IEnumerator _LeaveAnimation(float length)
		{
			if (Mathf.Approximately(_cg.alpha, 0)) yield break;

			_animating = true;
			float t = 0f;
			if (length <= 0.00001f) t = 999f;
			float startTime = Time.unscaledTime;

			var sd = _rect.sizeDelta;
			var startY = sd.y;
			sd.y = startY;
			_cg.alpha = 1f;
			_cg.blocksRaycasts = false;
			_rect.sizeDelta = sd;
			
			while (t < 1f)
			{
				sd.y = startY * (1f-Out(t));
				_cg.alpha = 1f-Out(t);
				_rect.sizeDelta = sd;
				t = (Time.unscaledTime - startTime) / length;
				yield return new WaitForEndOfFrame();
			}

			sd.y = 0f;
			_cg.alpha = 0f;
			_cg.blocksRaycasts = false;
			_rect.sizeDelta = sd;
			_animating = false;
		}
	}
}
