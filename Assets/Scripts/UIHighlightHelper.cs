using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public static class UIHighlightHelper
{
    public static IEnumerator Flash(Image img, Color color, float duration)
    {
        if (img == null) yield break;

        var original = img.color;
        img.color = color;
        yield return new WaitForSeconds(duration);
        img.color = original;
    }
}
