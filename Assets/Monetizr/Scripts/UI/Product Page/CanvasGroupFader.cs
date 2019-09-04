using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Monetizr.UI
{
    public class CanvasGroupFader : MonoBehaviour
    {
        private CanvasGroup _cg;
        private bool easing = false;

        private void Start()
        {
            _cg = GetComponent<CanvasGroup>();
        }

        public void DoEase(float length, float to, bool killEase = false)
        {
            if (killEase) StopEase();
            if (!easing)
            {
                StartCoroutine(FadeTo(length, to));
            }
        }

        public void StopEase()
        {
            easing = false;
            StopCoroutine("FadeTo");
        }

        IEnumerator FadeTo(float length, float to)
        {
            easing = true;
            float time = 0;
            float speed = 1f / length;
            AnimationCurve curve = AnimationCurve.Linear(0, _cg.alpha, 1, to);
            while (time < 1f)
            {
                time += speed * Time.deltaTime;
                _cg.alpha = curve.Evaluate(time);

                if (easing == false) yield break;

                yield return null;
            }

            time = 1f;
            _cg.alpha = curve.Evaluate(time);
            easing = false;
            yield return null;
        }
    }
}
