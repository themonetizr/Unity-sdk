using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Monetizr.UI
{
	public class EmphasisLineAnimator : MonoBehaviour
	{
		private RectTransform _rect;
		private CanvasGroup _group;
		private bool easing = false;

		private void Start()
		{
			_group = GetComponent<CanvasGroup>();
			_rect = GetComponent<RectTransform>();
		}

		public void DoEase(float length, float toScale, float toAlpha, bool killEase = false)
		{
			if (killEase) StopEase();
			if(!easing)
			{
				StartCoroutine(EaseTo(length, toScale, toAlpha));
			}
		}
		
		public void StopEase()
		{
			easing = false;
			StopCoroutine("EaseTo");
		}

		IEnumerator EaseTo(float length, float toScale, float toAlpha)
		{
			easing = true;
			float time = 0;
			float speed = 1f / length;
			AnimationCurve sizeCurve = AnimationCurve.EaseInOut(0, _rect.localScale.x, 1, toScale);
			AnimationCurve alphaCurve = AnimationCurve.Linear(0, _group.alpha, 1, toAlpha);
			
			while(time < 1f)
			{
				time += speed * Time.unscaledDeltaTime;
				Vector3 newScale = _rect.localScale;
				newScale.x = sizeCurve.Evaluate(time);
				_rect.localScale = newScale;

				_group.alpha = alphaCurve.Evaluate(time);

				if (easing == false) yield break;

				yield return null;
			}

			time = 1f;
			Vector3 newScale2 = _rect.localScale;
			newScale2.x = sizeCurve.Evaluate(time);
			_rect.localScale = newScale2;
			
			_group.alpha = alphaCurve.Evaluate(time);
			
			easing = false;
			yield return null;
		}

		public void Set(float toScale, float toAlpha)
		{
			Vector3 newScale2 = _rect.localScale;
			newScale2.x = toScale;
			_rect.localScale = newScale2;

			_group.alpha = toAlpha;
		}
	}
}