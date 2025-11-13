using TMPro;
using UnityEngine;

public class ScreenController : MonoBehaviour
{
    [SerializeField] bool _screenStartsOpen = true;
    CanvasGroup _canvas;

    [Header("Indication")]
    [SerializeField] TMP_Text _indicatorText;

    [Header("Screen visuals")]
    [SerializeField] CanvasGroup _visuals;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _canvas = GetComponent<CanvasGroup>();

        if (!_screenStartsOpen)
        {
            transform.localPosition = new Vector3(-128, 0, 0);
            _canvas.alpha = 0;
            _canvas.blocksRaycasts = false;
            _canvas.interactable = false;
        }

        SetIndicatorText("");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenScreen(bool delay)
    {
        LeanTween.moveLocalX(gameObject, 0, 0.2f).setOnStart(() =>
        {
            LeanTween.alphaCanvas(_canvas, 1, 0.2f).setOnStart(() =>
            {
                _canvas.interactable = true;
                if (_visuals)
                {
                    _visuals.alpha = 0;
                    LeanTween.alphaCanvas(_visuals, 1, 0.2f).setDelay(0.1f);
                }
            });
        }).setDelay(delay ? 0.2f : 0).setOnComplete(() =>
        {
            _canvas.blocksRaycasts = true;
            
        });
    }

    public void ReturnScreen(bool delay)
    {
        LeanTween.moveLocalX(gameObject, -128, 0.2f).setOnStart(() =>
        {
            LeanTween.alphaCanvas(_canvas, 0, 0.2f);
        }).setDelay(delay ? 0.2f : 0).setOnComplete(() =>
        {
            _canvas.blocksRaycasts = false;
            _canvas.interactable = false;
        });
    }

    public void SelectScreen(bool delay)
    {
        LeanTween.moveLocalX(gameObject, 128, 0.2f).setOnStart(() =>
        {
            LeanTween.alphaCanvas(_canvas, 0, 0.2f);
        }).setDelay(delay ? 0.2f : 0).setOnComplete(() =>
        {
            _canvas.blocksRaycasts = false;
            _canvas.interactable = false;
        });
    }

    public void SetIndicatorText(string text)
    {
        _indicatorText.text = text;
        _indicatorText.GetComponent<CanvasGroup>().alpha = 0;

        LeanTween.cancel(_indicatorText.gameObject);
        LeanTween.value(0, 1, 0.2f).setOnStart(() =>
        {
            LeanTween.alphaCanvas(_indicatorText.GetComponent<CanvasGroup>(), 1, 0.2f);
        }).setOnUpdate((float value) =>
        {
            _indicatorText.rectTransform.anchoredPosition = Vector2.Lerp(new(-224, 0), new(-96, 0), value);
        });
    }
}
