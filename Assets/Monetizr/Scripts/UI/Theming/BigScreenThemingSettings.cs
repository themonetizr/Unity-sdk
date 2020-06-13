using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

namespace Monetizr.UI.Theming
{
	[System.Serializable]
	public class BigScreenThemingSettings
	{
		[Range(0.4f, 0.83f)]
		[Tooltip("Sets the size of the offer window. A smaller scale might be desirable for desktop and a bigger size for WebGL.")]
		public float overlayScale = 0.7f;
		
		[Header("Coloring settings")]
		private bool _applyColorToMainText = true;
		public bool applyColorToButtonText = true;
		public bool applyColorToBottomButtons = true;
		public bool applyColorToDropdownButtons = true;
		public bool applyColorToImageScrollButtons = true;
		public bool applyColorToCloseButton = true;

		[Header("Button sprite overrides")]
		public SpriteSwapBlock bottomButtonOverrides = new SpriteSwapBlock();
		public SpriteSwapBlock dropdownButtonOverrides = new SpriteSwapBlock();
		public SpriteSwapBlock imageScrollLeftOverrides = new SpriteSwapBlock();
		public SpriteSwapBlock imageScrollRightOverrides = new SpriteSwapBlock();
		public SpriteSwapBlock closeButtonOverrides = new SpriteSwapBlock();
		
		[Tooltip("Leave empty to not use large border (outline will stay)")]
		public Sprite borderSprite;
		[Tooltip("Leave empty to not use custom full-size window background")]
		public Sprite backgroundSprite;

		[Header("Font overrides")]
		public Font bodyFont;
		public Font headerFont;
		public Font buttonFont;
		public Font smallTextFont;

		public bool ColoringAllowed(IThemable themable)
		{
			var t = themable as ThemableText;
			if (t != null)
				return ColoringAllowed(t.widgetType);

			var b = themable as ThemableImage;
			if (b != null)
				return ColoringAllowed(b.widgetType);

			return true;
		}
		
		public bool ColoringAllowed(Widget.WidgetType w)
		{
			switch (w)
			{
				case Widget.WidgetType.NotApplicable:
					return true;
				case Widget.WidgetType.Text:
					return _applyColorToMainText;
				case Widget.WidgetType.ButtonText:
					return applyColorToButtonText;
				case Widget.WidgetType.BottomButton:
					return applyColorToBottomButtons;
				case Widget.WidgetType.VariantButton:
					return applyColorToDropdownButtons;
				case Widget.WidgetType.LeftScrollButton:
					return applyColorToImageScrollButtons;
				case Widget.WidgetType.RightScrollButton:
					return applyColorToImageScrollButtons;
				case Widget.WidgetType.CloseButton:
					return applyColorToCloseButton;
				case Widget.WidgetType.WindowBorder:
					return false;
				case Widget.WidgetType.WindowBackground:
					return false;
				default:
					throw new ArgumentOutOfRangeException("w", w, null);
			}
		}

		public void ApplyFont(ThemableFont themable)
		{
			switch (themable.textType)
			{
				case ThemableFont.TextType.Body:
					if(bodyFont != null)
						themable.Apply(bodyFont);
					break;
				case ThemableFont.TextType.Button:
					if(buttonFont != null)
						themable.Apply(buttonFont);
					break;
				case ThemableFont.TextType.Header:
					if(headerFont != null)
						themable.Apply(headerFont);
					break;
				case ThemableFont.TextType.SmallText:
					if(smallTextFont != null)
						themable.Apply(smallTextFont);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public void CheckAndApplySpriteOverrides(IThemable themable)
		{
			var t = themable as ThemableText;
			if (t != null)
			{
				// Does not apply to text.
				return;
			}

			var b = themable as ThemableImage;
			if (b != null)
			{
				var button = b.GetComponent<Selectable>();
				switch (b.widgetType)
				{
					case Widget.WidgetType.NotApplicable:
						break;
					case Widget.WidgetType.Text:
						break;
					case Widget.WidgetType.ButtonText:
						break;
					case Widget.WidgetType.BottomButton:
						if (button != null)
							bottomButtonOverrides.Apply(button);
						break;
					case Widget.WidgetType.VariantButton:
						if (button != null)
							dropdownButtonOverrides.Apply(button);
						break;
					case Widget.WidgetType.LeftScrollButton:
						if (button != null)
							imageScrollLeftOverrides.Apply(button);
						break;
					case Widget.WidgetType.RightScrollButton:
						if (button != null)
							imageScrollRightOverrides.Apply(button);
						break;
					case Widget.WidgetType.CloseButton:
						if (button != null)
							closeButtonOverrides.Apply(button);
						break;
					case Widget.WidgetType.WindowBorder:
						if(borderSprite != null)
							b.Image.sprite = borderSprite;
						b.gameObject.SetActive(borderSprite != null);
						break;
					case Widget.WidgetType.WindowBackground:
						if(backgroundSprite != null)
							b.Image.sprite = backgroundSprite;
						b.gameObject.SetActive(backgroundSprite != null);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}
	}

	[System.Serializable]
	public class SpriteSwapBlock
	{
		public Sprite normalSprite;
		public Sprite highlightedSprite;
		public Sprite pressedSprite;
		public Sprite disabledSprite;

		public void Apply(Selectable b)
		{
			var ss = b.spriteState;
			if(normalSprite != null)
				b.GetComponent<Image>().sprite = normalSprite;
			if(highlightedSprite != null)
				ss.highlightedSprite = highlightedSprite;
			if(pressedSprite != null)
				ss.pressedSprite = pressedSprite;
			if(disabledSprite != null)
				ss.disabledSprite = disabledSprite;
			b.spriteState = ss;
		}
	}
}
