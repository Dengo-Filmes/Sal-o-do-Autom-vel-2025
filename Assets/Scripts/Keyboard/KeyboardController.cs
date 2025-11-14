using UnityEngine;

public class KeyboardController : MonoBehaviour
{
    InputSystem_Actions action;
    Vector2 _touchPosition;

    bool _shouldDrag = false;

    [SerializeField] LeanTweenType _tweenType;
    [SerializeField] float _tweenSpeed;

    private void Awake()
    {
        action = new();

        //_touchPosition = action.UI.Point.ReadValue<Vector2>();
        action.UI.Point.performed += ctx => _touchPosition = ctx.ReadValue<Vector2>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

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

    // Update is called once per frame
    void Update()
    {
        if(_shouldDrag)
            transform.position = _touchPosition;
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
