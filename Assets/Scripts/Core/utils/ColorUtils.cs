using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public static class ColorUtils
{

    public static void SetImageAlphaColor(Image img, float alpha)
    {
        img.color = new Color(img.color.r, img.color.g, img.color.b, alpha);
    }
}