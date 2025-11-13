using System;
using System.IO;
using TMPro;
using UnityEngine;

public class EraseFilesController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] TMP_InputField _passwordField;
    [SerializeField] GameObject _confirmButton;
    [SerializeField] ScreenController _menuScreen;
    [SerializeField] ScreenController _databaseScreen;

    [Header("Password")]
    [SerializeField] string _password = "Password";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _confirmButton.SetActive(!string.IsNullOrEmpty(_passwordField.text));
    }

    public void ConfirmErasure()
    {
        if (_passwordField.text == _password)
        {
            WarningPanelController.Instance.CallWarning("AVISO", "<b>Apagar dados?</b>", "<color=red>Sim", "Não");
            WarningPanelController.Instance.GetConfirmButton.onClick.AddListener(() => DeleteLeads());
        }
        else
        {
            _passwordField.text = string.Empty;
            WarningPanelController.Instance.CallWarning("AVISO", "Senha incorreta", "Fechar", "Sair");
            WarningPanelController.Instance.GetDeclineButton.onClick.AddListener(() => GetComponent<ScreenController>().ReturnScreen(false));
            WarningPanelController.Instance.GetDeclineButton.onClick.AddListener(() => _menuScreen.OpenScreen(true));
        }
    }

    void DeleteLeads()
    {
        if (Directory.Exists(Path.Combine(Application.dataPath, "DADOS SALVOS")))
            Directory.Delete(Path.Combine(Application.dataPath, "DADOS SALVOS"), true);
        else
            Debug.LogError("NO FOLDER FOUND IN " + Path.Combine(Application.dataPath, "DADOS SALVOS"));

        LeanTween.value(0, 1, 0.2f).setOnComplete(() =>
        {
            WarningPanelController.Instance.CallWarning("SUCESSO", "Dados apagados com sucesso. Uma entrada no log operacional foi adicionada.", "Fechar", "Sair");
            WarningPanelController.Instance.GetDeclineButton.onClick.AddListener(() => GetComponent<ScreenController>().ReturnScreen(false));
            WarningPanelController.Instance.GetDeclineButton.onClick.AddListener(() => _menuScreen.OpenScreen(true));
        });

        _passwordField.text = string.Empty;
        AddLogData();
    }

    public void ResetLeaderboard()
    {
        if (File.Exists(Path.Combine(Application.dataPath, "DADOS SALVOS", "leaderboard.txt")))
        {
            WarningPanelController.Instance.CallWarning("AVISO", "Esta ação apagará os dados de placar de líderes. Prosseguir?", "<color=red>Sim", "Não");
            WarningPanelController.Instance.GetConfirmButton.onClick.AddListener(() => DeleteLeaderboard());
        }
        else
        {
            WarningPanelController.Instance.CallWarning("AVISO", "Não há placar de líderes", "Fechar", "Sair");
            WarningPanelController.Instance.GetDeclineButton.onClick.AddListener(() => _databaseScreen.ReturnScreen(false));
            WarningPanelController.Instance.GetDeclineButton.onClick.AddListener(() => _menuScreen.OpenScreen(true));
        }
    }

    void DeleteLeaderboard()
    {
        if (File.Exists(Path.Combine(Application.dataPath, "DADOS SALVOS", "leaderboard.txt")))
            File.Delete(Path.Combine(Application.dataPath, "DADOS SALVOS", "leaderboard.txt"));

        LeanTween.value(0, 1, 0.2f).setOnComplete(() =>
        {
            WarningPanelController.Instance.CallWarning("SUCESSO", "Dados apagados com sucesso.", "Fechar", "Sair");
            WarningPanelController.Instance.GetDeclineButton.onClick.AddListener(() => _databaseScreen.ReturnScreen(false));
            WarningPanelController.Instance.GetDeclineButton.onClick.AddListener(() => _menuScreen.OpenScreen(true));
        });
    }

    void AddLogData()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "log_operacional.txt");

        using (StreamWriter sw = new StreamWriter(filePath, true))
        {
            sw.WriteLine("Arquivos apagados em: " + DateTime.Now);
        }
    }
}
