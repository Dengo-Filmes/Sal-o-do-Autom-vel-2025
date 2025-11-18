using UnityEngine;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{
    [Header("Referências UI")]
    public GameObject menuPanel;
    public Button openButton;
    public Button exitButton;

    [Header("Referência do Path")]
    public PathController pathController;

    void Start()
    {
        menuPanel.SetActive(false);

        openButton.onClick.AddListener(OpenPanel);
        exitButton.onClick.AddListener(ClosePanel);
    }

    public void OpenPanel()
    {
        menuPanel.SetActive(true);

        // Oculta o path quando o painel abre
        if (pathController != null)
        {
            pathController.SetPathVisibility(false);
        }
    }

    public void ClosePanel()
    {
        menuPanel.SetActive(false);

        // Mostra o path quando o painel fecha
        if (pathController != null)
        {
            pathController.SetPathVisibility(true);
        }
    }

    void OnDestroy()
    {
        openButton.onClick.RemoveListener(OpenPanel);
        exitButton.onClick.RemoveListener(ClosePanel);
    }
}