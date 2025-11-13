using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WarningPanelController : MonoBehaviour
{
    public static WarningPanelController Instance;
    [SerializeField] ScreenController _mainMenu;

    [Header("Visuals")]
    [SerializeField] RectTransform _warningHolder;

    [Header("Texts")]
    [SerializeField] TMP_Text _warningText;
    [SerializeField] TMP_Text _titleText;

    [SerializeField] Button _confirmButton;
    [SerializeField] Button _declineButton;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OpenWarningPanel()
    {
        CanvasGroup canvas = GetComponent<CanvasGroup>();

        GetConfirmButton.onClick.RemoveAllListeners();
        GetDeclineButton.onClick.RemoveAllListeners();

        LeanTween.alphaCanvas(canvas, 1, 0.2f).setOnComplete(() =>
        {
            LeanTween.value(0, 1, 0.2f).setOnUpdate((float value) =>
            {
                _warningHolder.anchoredPosition = Vector2.Lerp(new(-128, 0), Vector2.zero, value);
                _warningHolder.GetComponent<CanvasGroup>().alpha = value;
            }).setOnComplete(() =>
            {
                _warningHolder.GetComponent<CanvasGroup>().blocksRaycasts = true;
                _warningHolder.GetComponent<CanvasGroup>().interactable = true;
            });

            canvas.blocksRaycasts = true;
            canvas.interactable = true;
        });
    }

    public void CloseWarningPanel(bool confirm)
    {
        CanvasGroup canvas = GetComponent<CanvasGroup>();

        LeanTween.value(1, 0, 0.2f).setOnUpdate((float value) =>
        {
            _warningHolder.anchoredPosition = Vector2.Lerp(new(confirm ? 128 : -128, 0), Vector2.zero, value);
            _warningHolder.GetComponent<CanvasGroup>().alpha = value;
        }).setOnComplete(() =>
        {
            _warningHolder.GetComponent<CanvasGroup>().blocksRaycasts = false;
            _warningHolder.GetComponent<CanvasGroup>().interactable = false;
        }).setOnComplete(() =>
        {
            LeanTween.alphaCanvas(canvas, 0, 0.2f).setOnComplete(() =>
            {
                canvas.blocksRaycasts = false;
                canvas.interactable = false;
            });
        });
    }

    public Button GetConfirmButton { get { return _confirmButton; } }
    public Button GetDeclineButton { get  { return _declineButton; } }

    public void CallWarning(string warningText)
    {
        OpenWarningPanel();

        _titleText.text = "AVISO";
        _warningText.text = warningText;

        _confirmButton.GetComponentInChildren<TMP_Text>().text = "CONFIRMAR";
        _declineButton.GetComponentInChildren<TMP_Text>().text = "RECUSAR";
    }

    public void CallWarning(string title, string warningText, string confirm, string decline)
    {
        OpenWarningPanel();

        _titleText.text = title;
        _warningText.text = warningText;

        _confirmButton.GetComponentInChildren<TMP_Text>().text = confirm;
        _declineButton.GetComponentInChildren<TMP_Text>().text = decline;

        if(_confirmButton.GetComponentInChildren<TMP_Text>().text == "" || _declineButton.GetComponentInChildren<TMP_Text>().text == "")
        {
            _confirmButton.transform.parent.gameObject.SetActive(false);
        }
        else _confirmButton.transform.parent.gameObject.SetActive(true);
    }

    public void ReturnToMenu()
    {
        _mainMenu.OpenScreen(true);
    }
}
