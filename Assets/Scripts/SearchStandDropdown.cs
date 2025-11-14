using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class DropdownStandSearch : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public MapPanZoom2D map;

    [Header("Configurações")]
    public float searchZoom = 2f;
    public RectTransform arrowPrefab;
    public RectTransform mapTransform;
    private Dictionary<string, List<Stand2D>> groupedStands;

    void Start()
    {
        dropdown.ClearOptions();
        dropdown.captionText.text = "( o que procura? )";
        dropdown.value = -1;
        dropdown.RefreshShownValue();

        List<Stand2D> allStands = FindObjectsOfType<Stand2D>(true).ToList();

        groupedStands = allStands
            .GroupBy(s => s.standName)
            .ToDictionary(g => g.Key, g => g.ToList());

        // Ordena alfabeticamente (A - Z)
        List<string> sortedNames = groupedStands.Keys.OrderBy(name => name).ToList();

        dropdown.AddOptions(sortedNames);

        dropdown.onValueChanged.AddListener(OnDropdownSelect);
    }


    void OnDropdownSelect(int index)
    {
        if (index < 0) return;

        string standName = dropdown.options[index].text;

        if (!groupedStands.ContainsKey(standName))
            return;

        List<Stand2D> stands = groupedStands[standName];

        ArrowManager.Instance?.ClearAll();

        if (stands.Count == 1)
        {
            // 1 stand usa o focus normal
            Stand2D s = stands[0];
            map.FocusOnStand(s.GetComponent<RectTransform>(), s.focusZoom);
        }
        else
        {
            // 1 stand voltar zoom inicial SEM zoom de foco
            map.ResetCamera();
            Debug.Log($"Vários stands encontrados, resetando zoom para 1.");
        }

        // cria setas em todos os stands
        foreach (var stand in stands)
        {
            ArrowIndicatorController.Create(
                arrowPrefab,
                mapTransform,
                stand.GetComponent<RectTransform>()
            );

            var img = stand.GetComponent<UnityEngine.UI.Image>();
            if (img != null)
                StartCoroutine(UIHighlightHelper.Flash(img, Color.cyan, 0.4f));
        }
    }

}
