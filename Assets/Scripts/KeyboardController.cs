using UnityEngine;

public class KeyboardController : MonoBehaviour
{
    InputSystem_Actions action;
    Vector2 _touchPosition;
    bool _shouldDrag = false;

    [SerializeField] LeanTweenType _tweenType;
    [SerializeField] float _tweenSpeed;

    [Header("Configurações de Limite")]
    [SerializeField] Vector2 startPosition = new Vector2(0f, 800f); // Posição inicial
    [SerializeField] float margin = 50f; // Margem de segurança

    private RectTransform keyboardRect;
    private Canvas parentCanvas;
    private Camera canvasCamera;
    private Vector2 keyboardSize;

    private void Awake()
    {
        action = new();
        action.UI.Point.performed += ctx => _touchPosition = ctx.ReadValue<Vector2>();

        keyboardRect = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
        canvasCamera = parentCanvas.worldCamera;
        keyboardSize = keyboardRect.rect.size * parentCanvas.scaleFactor;

        // Define posição inicial
        keyboardRect.localPosition = startPosition;
    }

    public void OpenCloseKeyboard(bool open)
    {
        if (_shouldDrag) return;
        CanvasGroup childCanvas = transform.GetChild(0).GetComponent<CanvasGroup>();

        LeanTween.alphaCanvas(childCanvas, open ? 1 : 0, _tweenSpeed).setEase(_tweenType).setOnStart(() =>
        {
            LeanTween.moveLocalX(childCanvas.gameObject, open ? 0 : -45, _tweenSpeed).setEase(_tweenType);
        }).setOnComplete(() =>
        {
            childCanvas.blocksRaycasts = open;
        }).setDelay(0.05f);
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
                keyboardRect.parent as RectTransform,
                _touchPosition,
                canvasCamera,
                out Vector2 localPoint
            );

            // Aplica limites mas mantém a posição dentro deles
            localPoint = ApplyBounds(localPoint);

            keyboardRect.localPosition = localPoint;
        }
    }

    Vector2 ApplyBounds(Vector2 targetPosition)
    {
        Rect parentRect = (keyboardRect.parent as RectTransform).rect;

        // Limites considerando o tamanho do teclado
        float minX = parentRect.xMin + (keyboardSize.x / 2) + margin;
        float maxX = parentRect.xMax - (keyboardSize.x / 2) - margin;
        float minY = parentRect.yMin + (keyboardSize.y / 2) + margin;
        float maxY = parentRect.yMax - (keyboardSize.y / 2) - margin;

        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

        return targetPosition;
    }

    

    // Método de emergência (se quiser manter)
    public void ForceResetPosition()
    {
        keyboardRect.localPosition = startPosition;
        _shouldDrag = false;
    }

    private void OnEnable() => action.Enable();
    private void OnDisable() => action.Disable();
}