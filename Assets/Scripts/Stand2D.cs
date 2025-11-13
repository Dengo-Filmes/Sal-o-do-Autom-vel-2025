using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class Stand2D : MonoBehaviour, IPointerClickHandler
{
    [Header("Configurações do Stand")]
    public string standName = "Stand sem nome";
    public float focusZoom = 4f;
    public Color highlightColor = Color.yellow;

    [Header("Seta (prefab)")]
    public RectTransform arrowPrefab;    // arraste o prefab da seta
    public RectTransform mapTransform;   // arraste o painel do mapa

    private Color originalColor;
    private UnityEngine.UI.Image image;

    void Awake()
    {
        image = GetComponent<UnityEngine.UI.Image>();
        if (image != null)
            originalColor = image.color;

        // se não tiver setado no inspetor, tenta achar o painel do mapa
        if (mapTransform == null)
        {
            var mp = FindObjectOfType<MapPanZoom2D>();
            if (mp != null)
                mapTransform = mp.GetComponent<RectTransform>();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Foca a câmera no stand
        var map = FindObjectOfType<MapPanZoom2D>();
        if (map != null)
            map.FocusOnStand(GetComponent<RectTransform>(), focusZoom);

        //  Remove todas as setas antigas antes de criar uma nova
        foreach (var oldArrow in FindObjectsOfType<ArrowIndicatorController>())
        {
            if (oldArrow != null)
                oldArrow.DestroyArrow();
        }

        // Cria a nova seta acima do stand clicado
        if (arrowPrefab != null && mapTransform != null)
        {
            ArrowIndicatorController.Create(arrowPrefab, mapTransform, GetComponent<RectTransform>());
        }
        else
        {
            Debug.LogWarning("? Arrow prefab ou mapTransform não configurados no Stand2D.");
        }

        // efeito visual de destaque
        if (image != null)
        {
            StopAllCoroutines();
            StartCoroutine(FlashHighlight());
        }

        Debug.Log($" Stand selecionado: {standName}");
    }

    IEnumerator FlashHighlight()
    {
        image.color = highlightColor;
        yield return new WaitForSeconds(0.25f);
        image.color = originalColor;
    }
}
