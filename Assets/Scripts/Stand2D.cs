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

    [SerializeField] Transform _irlTransform;

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
        SelectIRLStand();

        var map = FindObjectOfType<MapPanZoom2D>();
        if (map != null)
            map.FocusOnStand(this, focusZoom);

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

    public void SelectIRLStand()
    {
        RectTransform rect = GetComponent<RectTransform>();

        Vector3 worldPoint = ConvertUIToOtherCamera(
            rect,
            Camera.main,
            MapPanZoom2D.Instance.mapCamera
        );

        Vector3 rayDir = MapPanZoom2D.Instance.mapCamera.transform.forward * 15;
        Ray ray = new(worldPoint, rayDir);

        if(Physics.Raycast(ray, out RaycastHit hit, 50f, 1 << 6))
        {
            _irlTransform = hit.transform;
            Debug.Log("IRL Stand is " +  _irlTransform.name);
        }
    }

    public Vector3 ConvertUIToOtherCamera(RectTransform button, Camera uiCam, Camera otherCam)
    {
        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(uiCam, button.position);

        screenPos.z = otherCam.transform.position.y;
        Vector3 current = otherCam.ScreenToWorldPoint(screenPos);
        Vector3 newCurrent = new(current.x, 10, current.z);

        return newCurrent;
    }

    public Transform GetIRLTransform()
    {
        return _irlTransform;
    }
}
