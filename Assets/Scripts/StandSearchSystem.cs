using UnityEngine;
using TMPro;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class SearchStandUI : MonoBehaviour
{
    [Header("Referências")]
    public TMP_InputField inputField;
    public MapPanZoom2D map;

    [Header("Configurações")]
    public float searchZoom = 4f;
    public bool partialMatch = true;
    public int minSearchLength = 3;

    [Header("Seta (prefab)")]
    public RectTransform arrowPrefab;     // arraste o prefab da seta
    public RectTransform mapTransform;    // arraste o painel do mapa

    void Start()
    {
        if (inputField != null)
            inputField.onEndEdit.AddListener(OnSearch);
    }

    void OnSearch(string query)
    {
        if (string.IsNullOrEmpty(query)) return;

        query = query.Trim().ToLower();

        // exige no mínimo X letras
        if (query.Length < minSearchLength)
        {
            Debug.LogWarning($" Digite pelo menos {minSearchLength} letras para buscar um stand.");
            return;
        }

        // encontra TODOS os stands que correspondem
        var allStands = FindObjectsOfType<Stand2D>(true).ToList();

        List<Stand2D> foundStands;

        if (partialMatch)
            foundStands = allStands.Where(s => s.standName.ToLower().Contains(query)).ToList();
        else
            foundStands = allStands.Where(s => s.standName.ToLower() == query).ToList();

        //  Remove todas as setas antigas antes de criar novas
        foreach (var oldArrow in FindObjectsOfType<ArrowIndicatorController>())
        {
            if (oldArrow != null)
                oldArrow.DestroyArrow();
        }

        // Se encontrou algum
        if (foundStands.Count > 0)
        {
            Debug.Log($" Encontrados {foundStands.Count} stand(s): {string.Join(", ", foundStands.Select(s => s.standName))}");

            // Centraliza a câmera no primeiro apenas
            var first = foundStands[0];
            map.FocusOnStand(first.GetComponent<RectTransform>(), searchZoom);

            // Mostra seta em todos os stands encontrados
            foreach (var stand in foundStands)
            {
                if (arrowPrefab != null && mapTransform != null)
                {
                    ArrowIndicatorController.Create(arrowPrefab, mapTransform, stand.GetComponent<RectTransform>());
                }

                var img = stand.GetComponent<UnityEngine.UI.Image>();
                if (img != null)
                    StartCoroutine(Highlight(img));
            }
        }
        else
        {
            Debug.LogWarning($" Nenhum stand encontrado com: {query}");
        }

        // limpa o input após a busca
        inputField.text = "";
    }

    IEnumerator Highlight(UnityEngine.UI.Image img)
    {
        var original = img.color;
        img.color = Color.cyan;
        yield return new WaitForSeconds(0.4f);
        img.color = original;
    }
}
