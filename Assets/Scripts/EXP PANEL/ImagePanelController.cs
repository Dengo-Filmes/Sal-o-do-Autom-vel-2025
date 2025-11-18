using UnityEngine;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{
    public static PanelController Instance;

    [Header("Referências UI")]
    public GameObject menuPanel;
    public Button openButton;
    public Button exitButton;

    [Header("Referência do Path")]
    public PathController pathController;

    void Start()
    {
        Instance = this;

        menuPanel.SetActive(false);

        openButton.onClick.AddListener(OpenPanel);
        exitButton.onClick.AddListener(ClosePanel);
    }

    public void OpenPanel()
    {
        menuPanel.SetActive(true);

        if (pathController != null)
            pathController.SetPathVisibility(false);
    }

    public void ClosePanel()
    {
        menuPanel.SetActive(false);

        if (pathController != null)
            pathController.SetPathVisibility(true);
    }

    public void ForceCloseIfOpen()
    {
        if (menuPanel.activeSelf)
        {
            ClosePanel();
        }
    }

    void OnDestroy()
    {
        openButton.onClick.RemoveListener(OpenPanel);
        exitButton.onClick.RemoveListener(ClosePanel);
    }
}
