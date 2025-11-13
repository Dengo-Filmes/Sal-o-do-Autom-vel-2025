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
    public RectTransform arrowPrefab;
    public RectTransform mapTransform;

    private List<Stand2D> allStands;

    void Start()
    {
        allStands = FindObjectsOfType<Stand2D>(true).ToList();
        if (inputField != null)
            inputField.onEndEdit.AddListener(OnSearch);
    }

    void OnSearch(string query)
    {
        if (string.IsNullOrEmpty(query)) return;

        query = query.Trim().ToLower();

        if (query.Length < minSearchLength)
        {
            Debug.LogWarning($"Digite pelo menos {minSearchLength} letras para buscar um stand.");
            return;
        }

        List<Stand2D> foundStands;

        if (partialMatch)
            foundStands = allStands.Where(s => s.standName.ToLower().Contains(query)).ToList();
        else
            foundStands = allStands.Where(s => s.standName.ToLower() == query).ToList();

        // remove todas as setas anteriores
        ArrowManager.Instance?.ClearAll();

        if (foundStands.Count > 0)
        {
            Debug.Log($"Encontrados {foundStands.Count} stand(s): {string.Join(", ", foundStands.Select(s => s.standName))}");

            var first = foundStands[0];
            map.FocusOnStand(first.GetComponent<RectTransform>(), searchZoom);

            foreach (var stand in foundStands)
            {
                if (arrowPrefab != null && mapTransform != null)
                    ArrowIndicatorController.Create(arrowPrefab, mapTransform, stand.GetComponent<RectTransform>());

                var img = stand.GetComponent<UnityEngine.UI.Image>();
                if (img != null)
                    StartCoroutine(UIHighlightHelper.Flash(img, Color.cyan, 0.4f));
            }
        }
        else
        {
            Debug.LogWarning($"Nenhum stand encontrado com: {query}");
        }

        inputField.text = "";
    }
}
