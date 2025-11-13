using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class UploadDataController : MonoBehaviour
{
    [Header("Configurações do Google Script")]
    string _webUrl = "https://script.google.com/macros/s/AKfycbw-EXYTdOqioy0A--BwiclPou9JTiABhr82iLpExa5WZmHmL3anEXMbDQLRFOU-IMJb/exec";

    [Header("Parâmetros de Envio")]
    string parentFolder = "DADOS SALVOS"; // pasta fixa no Drive
    [SerializeField] string subFolder = "Test";       // pasta do jogador

    string localFolderName = "DADOS SALVOS"; // pasta local onde os arquivos estão


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UploadData()
    {
        string localFolderPath = Path.Combine(Application.dataPath, localFolderName);
        string[] files = Directory.GetFiles(localFolderPath, "*.*", SearchOption.TopDirectoryOnly);

        if (!Directory.Exists(localFolderPath) || files.Length <= 0)
        {
            WarningPanelController.Instance.CallWarning("AVISO", "Não foram encontrados dados", "Fechar", "Sair");
            WarningPanelController.Instance.GetDeclineButton.onClick.AddListener(() => GetComponent<ScreenController>().ReturnScreen(false));
            WarningPanelController.Instance.GetDeclineButton.onClick.AddListener(() => WarningPanelController.Instance.ReturnToMenu());
            return;
        }

        WarningPanelController.Instance.CallWarning("AVISO", "Deseja enviar todos os arquivos de cadastro para a nuvem?", "Enviar", "Cancelar");
        WarningPanelController.Instance.GetConfirmButton.onClick.AddListener(() => UploadAllFiles());
    }

    void UploadAllFiles()
    {
        LeanTween.value(0, 1, 0.2f).setOnComplete(() =>
        {
            StartCoroutine(UploadAllFilesCoroutine());
        });
    }

    private IEnumerator UploadAllFilesCoroutine()
    {
        string localFolderPath = Path.Combine(Application.dataPath, localFolderName);

        if (!Directory.Exists(localFolderPath))
        {
            Debug.LogError("Pasta local não encontrada: " + localFolderPath);
            yield break;
        }

        string[] files = Directory.GetFiles(localFolderPath, "*.*", SearchOption.TopDirectoryOnly).Where(f => !f.EndsWith(".meta", System.StringComparison.OrdinalIgnoreCase)).ToArray();

        if (files.Length == 0)
        {
            Debug.LogWarning("Nenhum arquivo encontrado para upload.");
            yield break;
        }

        Debug.Log($"Encontrados {files.Length} arquivos para upload.");

        for (int i = 0; i < files.Length; i++)
        {
            string fileName = Path.GetFileName(files[i]);

            // Ignorar arquivos .meta
            if (fileName.EndsWith(".meta", System.StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log($"Ignorando arquivo .meta: {fileName}");
                continue;
            }

            // Lê o arquivo e converte para Base64
            byte[] fileData = File.ReadAllBytes(files[i]);
            string base64Data = Convert.ToBase64String(fileData);

            Debug.Log($"Enviando {fileName} ({fileData.Length} bytes)");

            // Monta o formulário com dados em Base64
            WWWForm form = new WWWForm();
            form.AddField("folder", subFolder);
            form.AddField("file", $"{DateTime.Now:HHmmss}_{fileName}");
            form.AddField("data", base64Data); // <- substitui o AddBinaryData

            using (UnityWebRequest www = UnityWebRequest.Post(_webUrl, form))
            {
                Debug.Log($"Enviando arquivo: {fileName}");
                WarningPanelController.Instance.CallWarning("DADOS", $"Enviando arquivo... [{i + 1} de {files.Length}]", "", "");

                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Erro ao enviar {fileName}: {www.error}");
                    WarningPanelController.Instance.CloseWarningPanel(false);
                    yield return new WaitForSeconds(0.2f);

                    WarningPanelController.Instance.CallWarning("<color=red>ERRO", $"Ocorreu algum erro ao enviar os arquivos.\nVerifique sua conexão com a internet.", "Fechar", "Sair");
                    WarningPanelController.Instance.GetDeclineButton.onClick.AddListener(() => GetComponent<ScreenController>().ReturnScreen(false));
                    WarningPanelController.Instance.GetDeclineButton.onClick.AddListener(() => WarningPanelController.Instance.ReturnToMenu());

                    yield break;
                }
                else
                {
                    Debug.Log($"Arquivo '{fileName}' enviado com sucesso! Resposta: {www.downloadHandler.text}");
                    WarningPanelController.Instance.CloseWarningPanel(true);
                }
            }

            // Delay curto entre envios (boa prática para Apps Script)
            yield return new WaitForSeconds(0.5f);
        }

        // Fechando painéis de aviso e confirmando sucesso
        WarningPanelController.Instance.CloseWarningPanel(true);
        yield return new WaitForSeconds(0.2f);

        WarningPanelController.Instance.CallWarning("DADOS", "Dados enviados com sucesso!", "Fechar", "Sair");
        WarningPanelController.Instance.GetDeclineButton.onClick.AddListener(() => GetComponent<ScreenController>().ReturnScreen(false));
        WarningPanelController.Instance.GetDeclineButton.onClick.AddListener(() => WarningPanelController.Instance.ReturnToMenu());

        Debug.Log("Upload em lote concluído!");
    }
}
