using UnityEngine;
using TMPro;
using System;

public class TimeDisplay : MonoBehaviour
{
    public TMP_Text dayText;
    public TMP_Text timeText;
    public string timeFormat = "HH:mm"; 

    void Start()
    {
        UpdateTime();
        InvokeRepeating(nameof(UpdateTime), 1f, 1f);
    }

    void UpdateTime()
    {
        DateTime now = DateTime.Now;
        dayText.text = now.ToString("dd/MM/yyyy");
        timeText.text = now.ToString(timeFormat);
    }
}
