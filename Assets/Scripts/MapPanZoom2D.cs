using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class MapPanZoom2D : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    [Header("References")]
    public RectTransform mapTransform;
    public RectTransform containerRect;
    public Slider zoomSlider;
    public Canvas canvas;

    [Header("Configurações")]
    public float panSpeed = 1f;
    public float minZoom = 1f;
    public float maxZoom = 4f;
    public float focusLerpSpeed = 6f;
    public float inactivityTime = 10f;

    private float zoom = 1f;
    private Coroutine focusCoroutine;
    private bool isFocusing = false;
    private float inactivityTimer = 0f;

    void Start()
    {
        if (zoomSlider)
        {
            zoomSlider.minValue = minZoom;
            zoomSlider.maxValue = maxZoom;
            zoomSlider.value = 1f;
            zoomSlider.onValueChanged.AddListener(SetZoom);
        }
    }

    void Update()
    {
        inactivityTimer += Time.deltaTime;

        if (inactivityTimer >= inactivityTime && !isFocusing)
        {
            ResetMapToDefault();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isFocusing) StopAllFocusCoroutines();
        ResetInactivityTimer();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isFocusing) return;
        mapTransform.anchoredPosition += eventData.delta * panSpeed;
        ClampMapInsideBounds();
        ResetInactivityTimer();
    }

    public void SetZoom(float value)
    {
        if (isFocusing) return;
        zoom = value;
        mapTransform.localScale = Vector3.one * zoom;
        ClampMapInsideBounds();
        ResetInactivityTimer();
    }

    public void FocusOnStand(RectTransform standRect, float zoomTarget = 4f)
    {
        StopAllFocusCoroutines();
        focusCoroutine = StartCoroutine(FocusRoutine(standRect, zoomTarget));
        ResetInactivityTimer();
    }

    IEnumerator FocusRoutine(RectTransform standRect, float zoomTarget)
    {
        isFocusing = true;
        zoomTarget = Mathf.Clamp(zoomTarget, minZoom, maxZoom);

        Vector2 startAnchoredPos = mapTransform.anchoredPosition;
        float startZoom = zoom;

        if (zoomSlider != null)
            zoomSlider.value = zoomTarget;

        Vector2 standLocalPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            mapTransform,
            RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, standRect.position),
            canvas.worldCamera,
            out standLocalPos
        );

        Vector2 containerLocalCenter;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            mapTransform,
            RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, containerRect.position),
            canvas.worldCamera,
            out containerLocalCenter
        );

        Vector2 delta = (standLocalPos - containerLocalCenter) * zoomTarget;
        Vector2 targetAnchoredPos = startAnchoredPos - delta;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * focusLerpSpeed;
            float smoothT = Mathf.SmoothStep(0, 1, t);

            zoom = Mathf.Lerp(startZoom, zoomTarget, smoothT);
            mapTransform.localScale = Vector3.one * zoom;
            mapTransform.anchoredPosition = Vector2.Lerp(startAnchoredPos, targetAnchoredPos, smoothT);

            ClampMapInsideBounds();
            yield return null;
        }

        zoom = zoomTarget;
        mapTransform.localScale = Vector3.one * zoom;
        mapTransform.anchoredPosition = targetAnchoredPos;
        ClampMapInsideBounds();

        isFocusing = false;
        focusCoroutine = null;
    }

    private void ResetMapToDefault()
    {
        if (isFocusing) return;
        StopAllFocusCoroutines();

        // Limpa todas as setas quando o mapa resetar por inatividade
        ArrowManager.Instance?.ClearAll();

        focusCoroutine = StartCoroutine(ResetRoutine());
    }

    IEnumerator ResetRoutine()
    {
        isFocusing = true;
        Vector2 startPos = mapTransform.anchoredPosition;
        float startZoom = zoom;
        float t = 0f;

        Vector2 targetPos = Vector2.zero;
        float targetZoom = 1f;

        if (zoomSlider) zoomSlider.value = targetZoom;

        while (t < 1f)
        {
            t += Time.deltaTime * focusLerpSpeed;
            zoom = Mathf.Lerp(startZoom, targetZoom, Mathf.SmoothStep(0, 1, t));
            mapTransform.localScale = Vector3.one * zoom;
            mapTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, Mathf.SmoothStep(0, 1, t));
            ClampMapInsideBounds();
            yield return null;
        }

        zoom = targetZoom;
        mapTransform.localScale = Vector3.one * zoom;
        mapTransform.anchoredPosition = targetPos;

        isFocusing = false;
        inactivityTimer = 0f;
    }

    private void ClampMapInsideBounds()
    {
        if (mapTransform == null || containerRect == null) return;

        Vector2 mapSize = mapTransform.rect.size * mapTransform.localScale.x;
        Vector2 containerSize = containerRect.rect.size;

        Vector2 limit = (mapSize - containerSize) / 2f;
        limit.x = Mathf.Max(0, limit.x);
        limit.y = Mathf.Max(0, limit.y);

        Vector2 pos = mapTransform.anchoredPosition;
        pos.x = Mathf.Clamp(pos.x, -limit.x, limit.x);
        pos.y = Mathf.Clamp(pos.y, -limit.y, limit.y);

        mapTransform.anchoredPosition = pos;
    }

    private void StopAllFocusCoroutines()
    {
        if (focusCoroutine != null) StopCoroutine(focusCoroutine);
        focusCoroutine = null;
        isFocusing = false;
    }

    private void ResetInactivityTimer()
    {
        inactivityTimer = 0f;
    }
}
