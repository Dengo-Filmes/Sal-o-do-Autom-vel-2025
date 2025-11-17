using UnityEngine;

public class LimitPinchScale : MonoBehaviour
{
    public float minScale = 0.8f;
    public float maxScale = 2.5f;

    private RectTransform rect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    void LateUpdate()
    {
        Vector3 s = rect.localScale;
        s.x = Mathf.Clamp(s.x, minScale, maxScale);
        s.y = Mathf.Clamp(s.y, minScale, maxScale);
        rect.localScale = s;
    }
}
