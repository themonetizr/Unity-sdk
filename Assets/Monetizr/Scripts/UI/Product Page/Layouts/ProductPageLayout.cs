using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Monetizr.UI
{
	public abstract class ProductPageLayout : MonoBehaviour
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

		public GameObject lockOverlay;

		public CheckoutWindow checkoutWindow;
		private static readonly int Opened = Animator.StringToHash("Opened");

		
		public virtual void SetOpened(bool opened)
		{
			animator.SetBool(Opened, opened);

			if (!opened) return;
		}

		public bool IsOpen {get {return animator.GetBool(Opened);}} 

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

			if (checkoutWindow != null)
			{
				checkoutWindow.CloseWindow();
			}
		}

		public virtual void OnFinishedLoading()
		{
			//pass
		}

		public abstract void UpdateButtons();
		public abstract void UpdateButtons(int idx);
	}
}
