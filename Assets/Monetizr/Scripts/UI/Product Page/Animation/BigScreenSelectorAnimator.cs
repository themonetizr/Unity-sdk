using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Monetizr.UI
{
	public class BigScreenSelectorAnimator : MonoBehaviour
	{
		private bool _animating = false;
		public bool Animating
		{
			get { return _animating; }
		}
		public SelectionManagerBigScreen manager;

		public void Entry(SelectorOptionBigScreen selected)
		{
			if (_animating) return;
			manager.options.ForEach(x =>
			{
				if(x.gameObject.activeInHierarchy)
					x.animator.SetClosed();
			});
			StartCoroutine(_Entry(selected));
		}

		private IEnumerator _Entry(SelectorOptionBigScreen selected)
		{
			_animating = true;
			selected.animator.OpenAnimation(0.1f);
			foreach (var i in manager.options)
			{
				if (!i.gameObject.activeInHierarchy) continue;
				if (i == selected) continue;
				i.animator.OpenAnimation(0.2f);
				yield return new WaitForSecondsRealtime(0.05f);
			}

			while (manager.options.Exists(x => x.animator.Animating))
			{
				yield return null;
			}
			
			_animating = false;
		}

		public void ExitToNext(SelectorOptionBigScreen selected, float newX, float moveLength = 0.1f)
		{
			if (_animating) return;
			StartCoroutine(_ExitToNext(selected, newX, moveLength));
		}
		
		private IEnumerator _ExitToNext(SelectorOptionBigScreen selected, float newX, float moveLength = 0.1f)
		{
			_animating = true;
			selected.animator.SelectAnimation(0.10f);
			foreach (var i in manager.options)
			{
				if (!i.gameObject.activeInHierarchy) continue;
				if (i == selected) continue;
				i.animator.LeaveAnimation(0.15f);
				yield return new WaitForSecondsRealtime(0.03f);
			}

			while (manager.options.Exists(x => x.animator.Animating))
			{
				yield return null;
			}

			yield return new WaitForSecondsRealtime(0.05f);
			selected.animator.LeaveAnimation(moveLength * 0.7f);

			var curPos = manager.selectionListRect.anchoredPosition;
			var startX = curPos.x;
			var startTime = Time.unscaledTime;
			var t = 0f;

			while (t < 1f)
			{
				curPos.x = Mathf.Lerp(startX, newX, BigScreenSelectorOptionAnimator.Out(t));
				manager.selectionListRect.anchoredPosition = curPos;
				yield return null;
				t = (Time.unscaledTime - startTime) / moveLength;
			}

			curPos.x = newX;
			manager.selectionListRect.anchoredPosition = curPos;
			
			_animating = false;
		}
		
		public void Exit(SelectorOptionBigScreen selected)
		{
			if (_animating) return;
			StartCoroutine(_Exit(selected));
		}
		
		private IEnumerator _Exit(SelectorOptionBigScreen selected)
		{
			_animating = true;
			foreach (var i in manager.options)
			{
				if (!i.gameObject.activeInHierarchy) continue;
				if (i == selected)
					i.animator.LeaveAnimation(0.3f);
				else
					i.animator.LeaveAnimation(0.15f);
				yield return new WaitForSecondsRealtime(0.03f);
			}

			while (manager.options.Exists(x => x.animator.Animating))
			{
				yield return null;
			}
			
			_animating = false;
		}
	}
}
