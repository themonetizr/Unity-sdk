using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Monetizr.UI
{
	public class ProductPageLayout : MonoBehaviour
	{
		public enum Layout
		{
			None,
			Vertical,
			Horizontal,
			BigScreen
		};

		public Layout layoutKind = Layout.Vertical;
		
		public Text header;
		public Text description;
		public RectTransform descriptionScroll;
		public Text price;
		public GameObject originalPriceBlock;
		public Text originalPrice;
		
		public Button checkoutButton;
		public Text checkoutText;

		public ImageViewer imageViewer;
		public List<GameObject> alternateDropdowns;

		public Animator animator;
		public Animator inlineImageLoaderAnimator;
		private static readonly int Opened = Animator.StringToHash("Opened");

		public void SetOpened(bool opened)
		{
			animator.SetBool(Opened, opened);
		}

		public void OpenIfLayout(Layout kind)
		{
			SetOpened(kind == layoutKind);
		}

		public virtual void InitalizeDropdowns(bool singular)
		{
			alternateDropdowns.ForEach(x => x.SetActive(false));
		}

		public void ResetDescriptionPosition()
		{
			Vector2 cur = descriptionScroll.anchoredPosition;
			cur.y = 0f;
			descriptionScroll.anchoredPosition = cur;
		}
	}
}
