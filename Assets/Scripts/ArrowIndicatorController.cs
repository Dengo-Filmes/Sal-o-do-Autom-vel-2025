using UnityEngine;
using UnityEngine.UI;

public class ArrowIndicatorController : MonoBehaviour
{
    public RectTransform arrowRect;       // referência ao próprio objeto
    public RectTransform mapTransform;    // o MapImage
    public float offsetY = 120f;          // distância acima do stand

    private static ArrowIndicatorController instance; // singleton simples

    void Awake()
    {
        if (arrowRect == null)
            arrowRect = GetComponent<RectTransform>();

        instance = this;
        gameObject.SetActive(false); // invisível no início
    }

    /// <summary>
    /// Chama para mover e exibir a seta sobre um stand.
    /// </summary>
    public static void ShowAbove(RectTransform standRect)
    {
        if (instance == null || standRect == null) return;

        instance.MoveTo(standRect);
    }

    void MoveTo(RectTransform standRect)
    {
        // pega posição local relativa ao mapa
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            mapTransform,
            RectTransformUtility.WorldToScreenPoint(null, standRect.position),
            null,
            out localPos
        );

        // aplica posição acima do stand
        arrowRect.anchoredPosition = localPos + new Vector2(0, offsetY);

        // ativa seta
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);
    }

    public static void Hide()
    {
        if (instance != null)
            instance.gameObject.SetActive(false);
    }
}
