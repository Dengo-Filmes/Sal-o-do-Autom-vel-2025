using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ButtonStandCategory : MonoBehaviour
{
    public string categoryName;                   
    public MapPanZoom2D map;
    public RectTransform arrowPrefab;
    public RectTransform mapTransform;

    private Dictionary<string, List<Stand2D>> groupedStands;

    void Start()
    {
        List<Stand2D> allStands = FindObjectsOfType<Stand2D>(true).ToList();

        groupedStands = allStands
            .GroupBy(s => s.standName)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    public void ShowCategory()
    {
        if (!groupedStands.ContainsKey(categoryName))
        {
            Debug.LogWarning($"Categoria não encontrada: {categoryName}");
            return;
        }

        List<Stand2D> stands = groupedStands[categoryName];

        ArrowManager.Instance?.ClearAll();

        map.FocusOnStand(stands[0].GetComponent<RectTransform>(), 1f);

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
