using UnityEngine;
using System.Collections;

public class ArrowIndicatorController : MonoBehaviour
{
    private RectTransform arrowRect;
    private RectTransform target;
    private Coroutine pulseRoutine;

    private Vector2 basePositionTop; 
    public static ArrowIndicatorController Create(RectTransform prefab, RectTransform parent, RectTransform target)
    {
        var instance = Instantiate(prefab, parent);
        var controller = instance.GetComponent<ArrowIndicatorController>();
        controller.Setup(target);

        ArrowManager.Instance?.Register(controller);

        return controller;
    }

    void Setup(RectTransform target)
    {
        arrowRect = GetComponent<RectTransform>();
        this.target = target;

        arrowRect.SetParent(target.parent, false);

        UpdatePosition();

        if (pulseRoutine != null)
            StopCoroutine(pulseRoutine);

        pulseRoutine = StartCoroutine(Pulse());
    }

    void Update()
    {
        if (target != null)
            UpdatePosition();
    }

    void UpdatePosition()
    {
        if (arrowRect == null || target == null) return;

        Vector2 pos = target.anchoredPosition;
        pos.y += target.rect.height * 0.5f;   

        basePositionTop = pos;                

        arrowRect.anchoredPosition = pos;
        arrowRect.localRotation = Quaternion.identity;
    }

    IEnumerator Pulse()
    {
        float amplitude = 10f;
        float speed = 2f;

        while (true)
        {
            if (arrowRect == null || target == null) yield break;

            float offsetY = Mathf.Sin(Time.time * speed) * amplitude;

            arrowRect.anchoredPosition = basePositionTop + new Vector2(0f, offsetY);

            yield return null;
        }
    }

    public void DestroyArrow()
    {
        if (pulseRoutine != null)
            StopCoroutine(pulseRoutine);

        Destroy(gameObject);
    }
}
