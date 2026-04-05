using System;
using UnityEngine;

namespace VRCAvatarColorChanger
{
    public enum SelectionMode
    {
        ColorPick,
        Rect
    }

    [Serializable]
    public class ColorZone
    {
        public string name = "Zone";
        public bool enabled = true;
        public SelectionMode mode = SelectionMode.ColorPick;

        // ColorPick mode
        public Color sampleColor = Color.white;
        public float tolerance = 0.15f;

        // Rect mode (UV coordinates 0-1)
        public Rect uvRect = new Rect(0, 0, 1, 1);

        // Target
        public Color targetColor = Color.red;

        // 0 = use target V completely (flat/vivid), 1 = keep original V completely (preserve pattern)
        [Range(0f, 1f)]
        public float valueBlend = 0.85f;

        public bool ContainsPixel(Color pixelColor, int x, int y, int texWidth, int texHeight)
        {
            if (!enabled) return false;

            switch (mode)
            {
                case SelectionMode.ColorPick:
                    return IsColorMatch(pixelColor);
                case SelectionMode.Rect:
                    return IsInRect(x, y, texWidth, texHeight);
                default:
                    return false;
            }
        }

        private bool IsColorMatch(Color pixelColor)
        {
            float pH, pS, pV, sH, sS, sV;
            Color.RGBToHSV(pixelColor, out pH, out pS, out pV);
            Color.RGBToHSV(sampleColor, out sH, out sS, out sV);

            // Hue is circular (0-1), compute shortest distance
            float hDist = Mathf.Abs(pH - sH);
            if (hDist > 0.5f) hDist = 1f - hDist;

            float sDist = Mathf.Abs(pS - sS);
            float vDist = Mathf.Abs(pV - sV);

            float distance = Mathf.Sqrt(hDist * hDist + sDist * sDist + vDist * vDist);
            return distance <= tolerance;
        }

        private bool IsInRect(int x, int y, int texWidth, int texHeight)
        {
            float u = (float)x / texWidth;
            float v = (float)y / texHeight;
            return uvRect.Contains(new Vector2(u, v));
        }
    }
}
