using System;
using UnityEngine;

namespace Monetizr.UI
{
	public class CanvasGroupMirror : MonoBehaviour
	{
		private CanvasGroup _cg;
		public CanvasGroup other;

		private void Start()
		{
			_cg = GetComponent<CanvasGroup>();
			if (!_cg) enabled = false;
		}

		private void Update()
		{
			_cg.alpha = other.alpha;
			_cg.interactable = other.interactable;
			_cg.blocksRaycasts = other.blocksRaycasts;
		}
	}
}
