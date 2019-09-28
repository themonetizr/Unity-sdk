using UnityEngine;
using UnityEngine.UI;

namespace Monetizr.Utility
{
    public static class UIUtility
    {
        public static void ShowCanvasGroup(ref CanvasGroup cg)
        {
            cg.alpha = 1;
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }

        public static void HideCanvasGroup(ref CanvasGroup cg)
        {
            cg.alpha = 0;
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }

        public static Rect GetScreenCoordinates(RectTransform uiElement)
        {
            var worldCorners = new Vector3[4];
            uiElement.GetWorldCorners(worldCorners);
            var result = new Rect(
                          worldCorners[0].x,
                          worldCorners[0].y,
                          worldCorners[2].x - worldCorners[0].x,
                          worldCorners[2].y - worldCorners[0].y);
            return result;
        }

        public static Rect RectFromScreenTo720p(Rect r)
        {
            Rect outRect = r;
            outRect.x *= 720f / (float)Screen.width;
            outRect.y *= 1280f / (float)Screen.height;
            outRect.width *= 720f / (float)Screen.width;
            outRect.height *= 1280f / (float)Screen.height;
            return outRect;
        }

        public static Color ColorFromSprite(Sprite spr)
        {
            return ColorFromTexture(spr.texture);
        }

        public static Color ColorFromTexture(Texture2D tex)
        {
            int x = tex.width / 2;
            int y = tex.height / 2;
            Color c = Color.gray;
            for(int i=0;i<10;i++)
            {
                Color p = tex.GetPixel(x, y);
                if (p.a < 0.9f)
                {
                    Vector2 rp = Random.insideUnitCircle;
                    x = (int)(tex.width * rp.x);
                    y = (int)(tex.height * rp.y);
                    continue;
                }

                float pH, pS, pV, cH, cS, cV;
                Color.RGBToHSV(p, out pH, out pS, out pV);
                Color.RGBToHSV(c, out cH, out cS, out cV);

                pS = Mathf.Max(pS, 0.5f);
                c = Color.HSVToRGB(pH, pS, cV);
                break;
            }
            return c;
        }

        public static bool IsPortrait()
        {
            return Screen.height >= Screen.width;
        }

        public static int GetMaxScreenDimension()
        {
            return Mathf.Max(Screen.height, Screen.width);
        }

        public static int GetMinScreenDimension()
        {
            return Mathf.Min(Screen.height, Screen.width);
        }

        /// <summary>
        /// Converts the anchoredPosition of the first RectTransform to the second RectTransform,
        /// taking into consideration offset, anchors and pivot, and returns the new anchoredPosition
        /// https://forum.unity.com/threads/find-anchoredposition-of-a-recttransform-relative-to-another-recttransform.330560/
        /// </summary>
        public static Vector2 SwitchToRectTransform(RectTransform from, RectTransform to)
        {
            Vector2 localPoint;
            Vector2 fromPivotDerivedOffset = new Vector2(from.rect.width * from.pivot.x + from.rect.xMin, from.rect.height * from.pivot.y + from.rect.yMin);
            Vector2 screenP = RectTransformUtility.WorldToScreenPoint(null, from.position);
            screenP += fromPivotDerivedOffset;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(to, screenP, null, out localPoint);
            Vector2 pivotDerivedOffset = new Vector2(to.rect.width * to.pivot.x + to.rect.xMin, to.rect.height * to.pivot.y + to.rect.yMin);
            return to.anchoredPosition + localPoint - pivotDerivedOffset;
        }
    }
}

