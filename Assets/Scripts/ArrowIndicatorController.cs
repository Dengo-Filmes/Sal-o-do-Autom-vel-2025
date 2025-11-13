using UnityEngine;
using System.Collections;

public class ArrowIndicatorController : MonoBehaviour
{
    private RectTransform arrowRect;
    private RectTransform target;
    private Coroutine pulseRoutine;

    public static ArrowIndicatorController Create(RectTransform prefab, RectTransform parent, RectTransform target)
    {
        var instance = Instantiate(prefab, parent);
        var controller = instance.GetComponent<ArrowIndicatorController>();
        controller.Setup(target);
        return controller;
    }

    void Setup(RectTransform target)
    {
        arrowRect = GetComponent<RectTransform>();
        this.target = target;
        UpdatePosition();

        //  animação suave (opcional) — movimento vertical, sem rotação
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
        if (arrowRect != null && target != null)
        {
            // posiciona logo acima do stand
            Vector3 pos = target.anchoredPosition;
            pos.y += target.rect.height * 0.6f; // distância acima do stand
            arrowRect.anchoredPosition = pos;

            // seta parada, sem rotação
            arrowRect.localRotation = Quaternion.identity;
        }
    }

    IEnumerator Pulse()
    {
        Vector3 basePos = arrowRect.localPosition;
        float amplitude = 10f; // quanto ela sobe/desce
        float speed = 2f;      // velocidade da animação

        while (true)
        {
            float offsetY = Mathf.Sin(Time.time * speed) * amplitude;
            arrowRect.localPosition = basePos + new Vector3(0, offsetY, 0);
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
