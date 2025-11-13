using UnityEngine;

public class SettingsMenuController : MonoBehaviour
{
    [Header("Referências dos Totens")]
    public GameObject totem1;
    public GameObject totem2;

    [Header("Tela de Configuração")]
    public CanvasGroup settingsPanel;

    [Header("Animação")]
    public float fadeDuration = 0.3f;

    private bool isOpen = false;

    void Start()
    {
        // Garante que o menu começa fechado
        if (settingsPanel)
        {
            settingsPanel.alpha = 0;
            settingsPanel.blocksRaycasts = false;
            settingsPanel.interactable = false;
        }
    }

    // Chamado pelo botão de configurações (OnTimerEnd)
    public void ToggleSettings()
    {
        if (isOpen)
            CloseMenu();
        else
            OpenMenu();
    }

    public void OpenMenu()
    {
        if (settingsPanel == null) return;
        LeanTween.alphaCanvas(settingsPanel, 1f, fadeDuration);
        settingsPanel.blocksRaycasts = true;
        settingsPanel.interactable = true;
        isOpen = true;
    }

    public void CloseMenu()
    {
        if (settingsPanel == null) return;
        LeanTween.alphaCanvas(settingsPanel, 0f, fadeDuration);
        settingsPanel.blocksRaycasts = false;
        settingsPanel.interactable = false;
        isOpen = false;
    }

    public void SelectTotem1()
    {
        if (totem1) totem1.SetActive(true);
        if (totem2) totem2.SetActive(false);
        CloseMenu();
        Debug.Log("Totem 1 ativado!");
    }

    public void SelectTotem2()
    {
        if (totem1) totem1.SetActive(false);
        if (totem2) totem2.SetActive(true);
        CloseMenu();
        Debug.Log("Totem 2 ativado!");
    }
}
