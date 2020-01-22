using UnityEngine;
using UnityEngine.UI;

namespace Monetizr.UI.Theming
{
	public class ThemableFont : MonoBehaviour
	{
		public enum TextType
		{
			Body,
			Button,
			Header,
			SmallText
		}

		private Text _text;

		public Text Text
		{
			get
			{
				if (_text == null)
				{
					_text = GetComponent<Text>();
				}

				return _text;
			}
		}
		public TextType textType;

		public void Apply(Font font)
		{
			if (Text == null) return;
			Text.font = font;
		}
	}
}
