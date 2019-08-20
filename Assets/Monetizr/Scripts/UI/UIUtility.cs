using UnityEngine;
using UnityEngine.UI;

namespace Monetizr.Utility
{
    public static class UIUtilityScript
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

                c = Color.HSVToRGB(pH, pS, cV);
                break;
            }
            return c;
        }
    }
}

