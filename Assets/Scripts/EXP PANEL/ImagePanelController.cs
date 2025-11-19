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
        menuPanel.transform.localScale = Vector3.zero;

        openButton.onClick.AddListener(OpenPanel);
        exitButton.onClick.AddListener(ClosePanel);
    }

    public void OpenPanel()
    {
        menuPanel.SetActive(true);

        menuPanel.transform.localScale = Vector3.zero;

        LeanTween.scale(menuPanel, Vector3.one, 0.6f)
            .setEaseOutBack();

        if (pathController != null)
            pathController.SetPathVisibility(false);
    }

    public void ClosePanel()
    {
        LeanTween.scale(menuPanel, Vector3.zero, 0.45f)
            .setEaseInBack()
            .setOnComplete(() =>
            {
                menuPanel.SetActive(false);

                if (pathController != null)
                    pathController.SetPathVisibility(true);
            });
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
