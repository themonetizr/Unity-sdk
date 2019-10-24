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
			Acccent,
			Disabled
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
				case ColorType.Disabled:
					return DisabledColor;
				default:
					return Color.black;
			}
		}
		
		[SerializeField]
		private Color backgroundColor = new Color(0.09f, 0.13f, 0.17f);
		[SerializeField]
		private Color primaryTextColor = new Color(0.96f, 0.96f, 0.96f);
		[SerializeField]
		private Color secondaryTextColor = new Color(0.44f, 0.52f, 0.6f);
		[SerializeField]
		private Color accentColor = new Color(0.42f, 0.7f, 0.95f);
		[SerializeField]
		private Color disabledColor = new Color(0.38f, 0.38f, 0.38f);

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

		public Color DisabledColor
		{
			get { return disabledColor; }
		}

		internal void SetDefaultDarkTheme()
		{
			backgroundColor = new Color(0.09f, 0.13f, 0.17f);
			primaryTextColor = new Color(0.96f, 0.96f, 0.96f);
			secondaryTextColor = new Color(0.44f, 0.52f, 0.6f);
			accentColor = new Color(0.42f, 0.7f, 0.95f);
			disabledColor = new Color(0.38f, 0.38f, 0.38f);
		}
		
		internal void SetDefaultLightTheme()
		{
			backgroundColor = new Color(0.95f, 0.95f, 0.95f);
			primaryTextColor = new Color(0.13f, 0.13f, 0.13f);
			secondaryTextColor = new Color(0.6f, 0.6f, 0.6f);
			accentColor = new Color(0.42f, 0.7f, 0.95f);
			disabledColor = new Color(0.81f, 0.81f, 0.81f);
		}

		internal void SetDefaultBlackTheme()
		{
			backgroundColor = new Color(0.0f, 0.0f, 0.0f);
			primaryTextColor = new Color(1f, 1f, 1f);
			secondaryTextColor = new Color(0.655f, 0.655f, 0.655f);
			accentColor = new Color(0.878f, 0.035f, 0.231f);
			disabledColor = new Color(0.68f, 0.68f, 0.68f);
		}
	}
}
