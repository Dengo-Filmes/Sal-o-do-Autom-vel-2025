using UnityEngine;

public class KeyboardController : MonoBehaviour
{
    InputSystem_Actions action;
    Vector2 _touchPosition;
    bool _shouldDrag = false;

    [SerializeField] LeanTweenType _tweenType;
    [SerializeField] float _tweenSpeed;

    Canvas parentCanvas;
    RectTransform rectTransform;
    RectTransform parentRect;

    private void Awake()
    {
        action = new();
        action.UI.Point.performed += ctx => _touchPosition = ctx.ReadValue<Vector2>();

        rectTransform = GetComponent<RectTransform>();
        parentRect = rectTransform.parent as RectTransform;

        // pega o canvas onde ele está
        parentCanvas = GetComponentInParent<Canvas>();
    }

    public void OpenCloseKeyboard(bool open)
    {
        if (_shouldDrag) return;

        CanvasGroup childCanvas = transform.GetChild(0).GetComponent<CanvasGroup>();

        LeanTween.alphaCanvas(childCanvas, open ? 1 : 0, _tweenSpeed)
            .setEase(_tweenType)
            .setOnStart(() =>
            {
                LeanTween.moveLocalX(childCanvas.gameObject, open ? 0 : -45, _tweenSpeed)
                    .setEase(_tweenType);
            })
            .setOnComplete(() =>
            {
                childCanvas.blocksRaycasts = open;
            })
            .setDelay(0.05f);
    }

    public void DragKeyboard(bool drag)
    {
        _shouldDrag = drag;
    }

    void Update()
    {
        if (_shouldDrag)
        {
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect,             
                _touchPosition,         
                parentCanvas.worldCamera,  
                out Vector2 localPoint
            );

            rectTransform.localPosition = localPoint;
        }
    }

    private void OnEnable()
    {
        action.Enable();
    }

    private void OnDisable()
    {
        action.Disable();
    }
}
