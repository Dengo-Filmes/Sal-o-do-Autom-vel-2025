using UnityEngine;
using UnityEngine.EventSystems;

public class Stand2D : MonoBehaviour, IPointerClickHandler
{
    [Header("Configurações do Stand")]
    public string standName = "Stand sem nome";
    public float focusZoom = 4f;
    public Color highlightColor = Color.yellow;

    private Color originalColor;
    private UnityEngine.UI.Image image;

    void Awake()
    {
        image = GetComponent<UnityEngine.UI.Image>();
        if (image != null)
            originalColor = image.color;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var map = FindObjectOfType<MapPanZoom2D>();
        if (map != null)
        {
            map.FocusOnStand(GetComponent<RectTransform>(), focusZoom);
        }


        if (image != null)
        {
            StopAllCoroutines();
            StartCoroutine(FlashHighlight());
        }

        Debug.Log($" Stand selecionado: {standName}");
    }

    System.Collections.IEnumerator FlashHighlight()
    {
        image.color = highlightColor;
        yield return new WaitForSeconds(0.25f);
        image.color = originalColor;
    }
}
