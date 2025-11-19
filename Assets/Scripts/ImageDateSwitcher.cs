using UnityEngine;
using UnityEngine.UI;
using System;

public class PanelDailyImage : MonoBehaviour
{
    public Image targetImage;
    public Sprite image21;   // sexta-feira 21/11
    public Sprite image22;   // sábado 22/11 em diante

    private DateTime lastCheckedDate;

    void Start()
    {
        UpdateImageByDate();
        lastCheckedDate = DateTime.Now;

        // Verifica a mudança de dia automaticamente
        InvokeRepeating(nameof(CheckDateUpdate), 30f, 30f);
        // verifica a cada 30 segundos (pode aumentar para 60s se quiser)
    }

    void CheckDateUpdate()
    {
        if (DateTime.Now.Date != lastCheckedDate.Date)
        {
            UpdateImageByDate();
            lastCheckedDate = DateTime.Now;
        }
    }

    void UpdateImageByDate()
    {
        int day = DateTime.Now.Day;

        if (day <= 21)
        {
            targetImage.sprite = image21;
        }
        else
        {
            targetImage.sprite = image22;
        }
    }
}
