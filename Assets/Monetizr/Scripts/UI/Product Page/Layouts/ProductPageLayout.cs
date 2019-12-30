using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Monetizr.UI
{
	public class ProductPageLayout : MonoBehaviour
	{
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

		public virtual void InitalizeDropdowns(List<VariantsDropdown> mainDd, bool singular)
		{
			alternateDropdowns.ForEach(x => x.SetActive(false));
			if (singular) return;
			for (int i = 0; i < mainDd.Count; i++)
			{
				alternateDropdowns[i].SetActive(true);
				mainDd[i].Alternate.Add(alternateDropdowns[i].GetComponent<AlternateVariantsDropdown>());
			}
		}

		public void ResetDescriptionPosition()
		{
			Vector2 cur = descriptionScroll.anchoredPosition;
			cur.y = 0f;
			descriptionScroll.anchoredPosition = cur;
		}
	}
}
