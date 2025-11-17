using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;

public class Stand2D : MonoBehaviour, IPointerClickHandler
{
    [Header("Configurações do Stand")]
    public string standName = "Stand sem nome";
    public float focusZoom = 2f;

    [Header("Seta (prefab)")]
    public RectTransform arrowPrefab;
    public RectTransform mapTransform;

    [Header("Highlight")]
    public Color originalColor;  // agora público para outros scripts acessarem
    private Color highlightColor = new Color(255f / 255f, 20f / 255f, 4f / 255f, 100f / 255f);

    private Image image;

    [SerializeField] Transform _irlTransform;

    void Awake()
    {
        image = GetComponent<Image>();
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
            map.FocusOnStand(this, focusZoom);

        ArrowManager.Instance?.ClearAll();

        if (arrowPrefab != null && mapTransform != null)
        {
            ArrowIndicatorController.Create(
                arrowPrefab,
                mapTransform,
                GetComponent<RectTransform>()
            );
        }
        else
        {
            Debug.LogWarning("Arrow prefab ou mapTransform não configurados no Stand2D.");
        }

        if (image != null)
        {
            StopAllCoroutines();                  // evita acúmulo
            image.color = originalColor;          // reseta cor original
            StartCoroutine(UIHighlightHelper.Flash(image, highlightColor, 0.25f));
        }

        Debug.Log($"Stand selecionado: {standName}");
    }

    public Transform GetIRLTransform() => _irlTransform;
}
