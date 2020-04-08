using UnityEngine;
using UnityEngine.UI;
using Monetizr.Utility;

namespace Monetizr.UI.Widgets
{
	public class OfferListItem : MonoBehaviour
	{
		public Text nameText;
		public Image thumbnailImage;
		public GameObject lockedOverlay;
		private string _tag;
		private bool _locked = false;

		public void Initialize(ListProduct source, bool isLocked)
		{
			_tag = source.Tag;
			_locked = isLocked;
			nameText.text = source.Name;
			lockedOverlay.SetActive(_locked);
			source.Thumbnail.GetOrDownloadImage(img => thumbnailImage.sprite = UIUtility.CropSpriteToSquare(img));
		}

		public void Open()
		{
			MonetizrClient.Instance.ShowProductForTag(_tag, _locked);
		}
	}
}
