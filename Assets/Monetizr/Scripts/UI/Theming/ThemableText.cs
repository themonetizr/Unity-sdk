using System;
using UnityEngine;
using UnityEngine.UI;

namespace Monetizr.UI.Theming
{
	public class ThemableText : MonoBehaviour, IThemable
	{
		public ColorScheme.ColorType colorType;
		private Text _text;

		public Text Text
		{
			get
			{
				if(_text == null)
					_text = GetComponent<Text>();
				
				return _text;
			}
		}

		public void Apply(ColorScheme scheme)
		{
			if (Text == null) return;
			Text.color = scheme.GetColorForType(colorType);
		}
	}
}
