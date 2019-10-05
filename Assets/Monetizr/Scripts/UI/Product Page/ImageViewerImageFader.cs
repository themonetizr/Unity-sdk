using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Monetizr.UI
{
    public class ImageViewerImageFader : MonoBehaviour
    {
        public ImageViewer Viewer;

        public Image Image;
        public RectTransform RectTransform;

        private void Update()
        {
            if (!Viewer.IsOpen()) return;

            Vector2 posOnCenter
                = Utility.UIUtility.SwitchToRectTransform(RectTransform, Viewer.ScrollView);
            float diff = Mathf.Abs(posOnCenter.x);
            if (diff < 1f) diff = 0f;

            var distanceToDisappear = Screen.width * 2f;
            
            var newColor = Image.color;
            newColor.a = Mathf.Lerp(1f, 0f, diff / distanceToDisappear);
            Image.color = newColor;

            var newScale = Vector3.one * Mathf.Lerp(1f, 0.75f, diff / distanceToDisappear);
            RectTransform.localScale = newScale;
        }
    }
}