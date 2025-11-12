using UnityEngine;
using TMPro;
using System.Linq;

public class SearchStandUI : MonoBehaviour
{
    [Header("Referências")]
    public TMP_InputField inputField;
    public MapPanZoom2D map;

    [Header("Configurações")]
    public float searchZoom = 4f;  
    public bool partialMatch = true; 

    void Start()
    {
        if (inputField != null)
            inputField.onEndEdit.AddListener(OnSearch);
    }

    void OnSearch(string query)
    {
        if (string.IsNullOrEmpty(query)) return;

        query = query.Trim().ToLower();

        
        var stands = FindObjectsOfType<Stand2D>(true);

        Stand2D foundStand = null;

        if (partialMatch)
        {
            foundStand = stands.FirstOrDefault(s =>
                s.standName.ToLower().Contains(query));
        }
        else
        {
            foundStand = stands.FirstOrDefault(s =>
                s.standName.ToLower() == query);
        }

        if (foundStand != null)
        {
            Debug.Log($" Encontrado: {foundStand.standName}");
            map.FocusOnStand(foundStand.GetComponent<RectTransform>(), searchZoom);

            
            var img = foundStand.GetComponent<UnityEngine.UI.Image>();
            if (img != null)
                StartCoroutine(Highlight(img));
        }
        else
        {
            Debug.LogWarning($" Nenhum stand encontrado com: {query}");
        }

        
        inputField.text = "";
    }

    System.Collections.IEnumerator Highlight(UnityEngine.UI.Image img)
    {
        var original = img.color;
        img.color = Color.cyan;
        yield return new WaitForSeconds(0.4f);
        img.color = original;
    }
}
