using UnityEngine;

namespace Monetizr.UI.Theming
{
	[System.Serializable]
	public class ColorScheme
	{
		public enum ColorType
		{
			Background,
			PrimaryText,
			SecondaryText,
			Acccent
		};

		public Color GetColorForType(ColorType type)
		{
			switch (type)
			{
				case ColorType.Acccent:
					return AccentColor;
				case ColorType.Background:
					return BackgroundColor;
				case ColorType.PrimaryText:
					return PrimaryTextColor;
				case ColorType.SecondaryText:
					return SecondaryTextColor;
				default:
					return Color.black;
			}
		}
		
		[SerializeField]
		private Color backgroundColor = new Color(0f, 0f, 0f, 1f);
		[SerializeField]
		private Color primaryTextColor = new Color(1f, 1f, 1f, 1f);
		[SerializeField]
		private Color secondaryTextColor = new Color(0.655f, 0.655f, 0.655f, 1f);
		[SerializeField]
		private Color accentColor = new Color(0.878f, 0.035f, 0.231f, 1f);

		public Color BackgroundColor
		{
			get { return backgroundColor; }
		}

		public Color PrimaryTextColor
		{
			get { return primaryTextColor; }
		}

		public Color SecondaryTextColor
		{
			get { return secondaryTextColor; }
		}

		public Color AccentColor
		{
			get { return accentColor; }
		}

		internal void SetDefaultDarkTheme()
		{
			backgroundColor = new Color(0f, 0f, 0f, 1f);
			primaryTextColor = new Color(1f, 1f, 1f, 1f);
			secondaryTextColor = new Color(0.655f, 0.655f, 0.655f, 1f);
			accentColor = new Color(0.878f, 0.035f, 0.231f, 1f);
		}
		
		internal void SetDefaultLightTheme()
		{
			backgroundColor = new Color(1f, 1f, 1f, 1f);
			primaryTextColor = new Color(0f, 0f, 0f, 1f);
			secondaryTextColor = new Color(0.49f, 0.49f, 0.49f);
			accentColor = new Color(0.878f, 0.035f, 0.231f, 1f);
		}
	}
}
