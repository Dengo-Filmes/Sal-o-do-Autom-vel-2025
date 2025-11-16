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

    private Color unifiedHighlightColor = new Color(255f / 255f, 20f / 255f, 4f / 255f, 100f / 255f);

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
            Stand2D s = stands[0];
            map.FocusOnStand(s.GetComponent<RectTransform>(), s.focusZoom);
        }
        else
        {
            map.ResetCamera();
            Debug.Log($"Vários stands encontrados, resetando zoom para 1.");
        }

        foreach (var stand in stands)
        {
            ArrowIndicatorController.Create(
                arrowPrefab,
                mapTransform,
                stand.GetComponent<RectTransform>()
            );

            var img = stand.GetComponent<UnityEngine.UI.Image>();
            if (img != null)
                StartCoroutine(UIHighlightHelper.Flash(img, unifiedHighlightColor, 0.4f));
        }
    }
}
