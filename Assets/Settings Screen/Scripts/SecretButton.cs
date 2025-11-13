using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(EventTrigger))]
public class SecretButton : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] UnityEvent OnTimerEnd = new();
    [SerializeField] float holdTime = 2;
    float timer;
    bool holding = false;
    bool clicked = false;

    [Header("UI")]
    [SerializeField] Image roundDial;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CallEvent();

        ControlUI();
    }

    void ControlUI()
    {
        roundDial.fillAmount = timer / holdTime;
    }

    public void SetHoldButton(bool hold)
    {
        holding = hold;
    }

    void CallEvent()
    {
        timer = holding ? timer + Time.deltaTime : 0;
        if (timer > holdTime)
        {
            timer = holdTime;
            if (!clicked) OnTimerEnd.Invoke();
            clicked = true;
        }
        else clicked = false;
    }
}
