using System.Collections.Generic;
using UnityEngine;

public class ArrowManager : MonoBehaviour
{
    public static ArrowManager Instance;
    private readonly List<ArrowIndicatorController> activeArrows = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Register(ArrowIndicatorController arrow)
    {
        if (!activeArrows.Contains(arrow))
            activeArrows.Add(arrow);
    }

    public void ClearAll()
    {
        foreach (var arrow in activeArrows)
        {
            if (arrow != null)
                arrow.DestroyArrow();
        }
        activeArrows.Clear();
    }
}
