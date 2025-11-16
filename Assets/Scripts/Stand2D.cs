using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class Stand2D : MonoBehaviour, IPointerClickHandler
{
    [Header("Configurações do Stand")]
    public string standName = "Stand sem nome";
    public float focusZoom = 2f;

    [Header("Seta (prefab)")]
    public RectTransform arrowPrefab;
    public RectTransform mapTransform;

    private Color originalColor;
    private Color highlightColor = new Color(255f / 255f, 20f / 255f, 4f / 255f, 100f / 255f);
    private UnityEngine.UI.Image image;

    void Awake()
    {
        image = GetComponent<UnityEngine.UI.Image>();
        if (image != null)
            originalColor = image.color;

        if (mapTransform == null)
        {
            var mp = FindObjectOfType<MapPanZoom2D>();
            if (mp != null)
                mapTransform = mp.GetComponent<RectTransform>();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var map = FindObjectOfType<MapPanZoom2D>();
        if (map != null)
            map.FocusOnStand(GetComponent<RectTransform>(), focusZoom);

        ArrowManager.Instance?.ClearAll();

        if (arrowPrefab != null && mapTransform != null)
        {
            ArrowIndicatorController.Create(arrowPrefab, mapTransform, GetComponent<RectTransform>());
        }
        else
        {
            Debug.LogWarning("Arrow prefab ou mapTransform não configurados no Stand2D.");
        }

        if (image != null)
        {
            StopAllCoroutines();
            StartCoroutine(UIHighlightHelper.Flash(image, highlightColor, 0.25f));
        }

        Debug.Log($"Stand selecionado: {standName}");
    }
}
