using System;
using UnityEngine;
using UnityEngine.UI;

namespace Monetizr.UI.Theming
{
    public class ThemableImage : MonoBehaviour, IThemable
    {
        public ColorScheme.ColorType colorType;
        public Widget.WidgetType widgetType = Widget.WidgetType.NotApplicable;
        private Image _image;

        public Image Image
        {
            get
            {
                if(_image == null)
                    _image = GetComponent<Image>();
				
                return _image;
            }
        }

        public void Apply(ColorScheme scheme)
        {
            if (Image == null) return;
            Image.color = scheme.GetColorForType(colorType);
        }
    }
}