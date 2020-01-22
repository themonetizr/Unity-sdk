using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Monetizr.UI
{
    public class SelectionBarAnimator : MonoBehaviour
    {
        private RectTransform _rect;
        private bool easing = false;

        private void Start()
        {
            _rect = GetComponent<RectTransform>();
        }

        public void DoEase(float length, float to, bool killEase = false)
        {
            if (killEase) StopEase();
            if(!easing)
            {
                StartCoroutine(EaseToY(length, to));
            }
        }

        public void StopEase()
        {
            easing = false;
            StopCoroutine("EaseToY");
        }

        IEnumerator EaseToY(float length, float to)
        {
            easing = true;
            float time = 0;
            float speed = 1f / length;
            AnimationCurve curve = AnimationCurve.EaseInOut(0, _rect.anchoredPosition.y, 1, to);
            while(time < 1f)
            {
                time += speed * Time.unscaledDeltaTime;
                Vector2 newPos2 = _rect.anchoredPosition;
                newPos2.y = curve.Evaluate(time);
                _rect.anchoredPosition = newPos2;

                if (easing == false) yield break;

                yield return null;
            }

            time = 1f;
            Vector2 newPos = _rect.anchoredPosition;
            newPos.y = curve.Evaluate(time);
            _rect.anchoredPosition = newPos;
            easing = false;
            yield return null;
        }
    }
}