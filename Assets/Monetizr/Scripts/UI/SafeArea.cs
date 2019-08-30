using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Monetizr
{
    /// <summary>
    /// This script scales UI so notches do not interfere with it.
    /// Does not work in 2017.1 or newer, because uses Screen.safeArea API
    /// </summary>
    public class SafeArea : MonoBehaviour
    {
        RectTransform Panel;
        Rect LastSafeArea = new Rect(0, 0, 0, 0);

        void Awake()
        {
            Panel = GetComponent<RectTransform>();
            Refresh();
        }

#if UNITY_2017_2_OR_NEWER
        void Update()
        {
            Refresh();
        }

        void Refresh()
        {
            Rect safeArea = GetSafeArea();

            if (safeArea != LastSafeArea)
                ApplySafeArea(safeArea);
        }
#endif

        Rect GetSafeArea()
        {
#if UNITY_2017_2_OR_NEWER
            return Screen.safeArea;
#else
            return null;
#endif
        }

        void ApplySafeArea(Rect r)
        {
            LastSafeArea = r;

            // Convert safe area rectangle from absolute pixels to normalised anchor coordinates
            Vector2 anchorMin = r.position;
            Vector2 anchorMax = r.position + r.size;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;
            Panel.anchorMin = anchorMin;
            Panel.anchorMax = anchorMax;

            //Debug.LogFormat("New safe area applied to {0}: x={1}, y={2}, w={3}, h={4} on full extents w={5}, h={6}",
            //    name, r.x, r.y, r.width, r.height, Screen.width, Screen.height);
        }
    }
}