using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Opening")]
    [SerializeField] CanvasGroup _blackScreen;
    [SerializeField] CanvasGroup _plenoLogo;
    [SerializeField] CanvasGroup _bars;
    [SerializeField] CanvasGroup _mainLines;
    ScreenController _mainMenu;

    [Header("Version")]
    [SerializeField] TMP_Text _versionText;

    [Header("Application Settings")]
    [SerializeField] int _firstSceneIndex = 0;
    [SerializeField] ScreenController _userDataScreen;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _mainMenu = GetComponent<ScreenController>();

        _blackScreen.alpha = 1;
        _plenoLogo.alpha = 0;
        _bars.alpha = 0;
        _mainLines.alpha = 0;

        Opening();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Opening()
    {
        _versionText.text = $"{Application.productName} ver. {Application.version}";

        LeanTween.alphaCanvas(_blackScreen, 0, 0.2f).setOnComplete(() =>
        {
            LeanTween.alphaCanvas(_plenoLogo, 1, 0.2f).setOnComplete(() =>
            {
                LeanTween.alphaCanvas(_bars, 1, 0.2f);
                LeanTween.alphaCanvas(_mainLines, 1, 0.2f).setOnComplete(() =>
                {
                    _mainMenu.OpenScreen(false);
                }).setDelay(0.1f);
            });
        }).setDelay(0.2f);
    }

    public void OpenUserData()
    {
        WarningPanelController.Instance.CallWarning("<color=red>AVISO!</color> DADOS SENSÍVEIS", "Esta tela é direcionada ao <b>técnico responsável</b> pela montagem e aplicação deste programa.\nCaso não seja o técnico responsável, favor contatá-lo.\nAvançar?", "Avançar", "Retornar");
        WarningPanelController.Instance.GetConfirmButton.onClick.AddListener(() => _mainMenu.SelectScreen(false));
        WarningPanelController.Instance.GetConfirmButton.onClick.AddListener(() => _userDataScreen.OpenScreen(true));
    }

    public void RestartApplication()
    {
        WarningPanelController.Instance.CallWarning("REINICIAR", "Tem certeza de que quer <b>reiniciar</b> a aplicação?", "Sim", "Não");
        WarningPanelController.Instance.GetConfirmButton.onClick.AddListener(() => LoadScene(_firstSceneIndex));
    }

    public void KillApplication()
    {
        WarningPanelController.Instance.CallWarning("ENCERRAR", "Tem certeza de que quer <b>encerrar</b> a aplicação?", "Sim", "Não");
        WarningPanelController.Instance.GetConfirmButton.onClick.AddListener(() => KillGame());
    }

    public void CloseSettings()
    {
        LeanTween.alphaCanvas(_blackScreen, 1, 0.5f).setOnComplete(() =>
        {
            OpenSettingsController.Instance.root.SetActive(true);
            SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("Settings [Horizontal]"));
        });
    }

    void LoadScene(int index)
    {
        LeanTween.cancelAll();
        SceneManager.LoadScene(index);
    }

    void KillGame()
    {
        Application.Quit();
    }
}
