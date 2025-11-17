using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public static class UIHighlightHelper
{
    public static IEnumerator Flash(Image img, Color color, float duration)
    {
        if (img == null) yield break;

        Color original = img.color;

        img.color = color;
        yield return new WaitForSeconds(duration);

        if (img != null)
            img.color = original;
    }
}
