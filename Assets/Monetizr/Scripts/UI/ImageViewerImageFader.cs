using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Monetizr
{
    public class ImageViewerImageFader : MonoBehaviour
    {
        public ImageViewer Viewer;

        public Image Image;
        public RectTransform RectTransform;
        public float DistanceToDisappear = 500f;

        private void Update()
        {
            if (!Viewer.IsOpen()) return;

            Vector2 posOnCenter
                = Utility.UIUtilityScript.SwitchToRectTransform(RectTransform, Viewer.ScrollView);
            float diff = Mathf.Abs(posOnCenter.x);
            if (diff < 1f) diff = 0f;

            var newColor = Image.color;
            newColor.a = Mathf.Lerp(1f, 0f, diff / DistanceToDisappear);
            Image.color = newColor;

            var newScale = Vector3.one * Mathf.Lerp(1f, 0.75f, diff / DistanceToDisappear);
            RectTransform.localScale = newScale;
        }
    }
}