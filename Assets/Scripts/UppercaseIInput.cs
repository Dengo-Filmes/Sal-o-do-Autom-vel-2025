using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_InputField))]
public class UppercaseInput : MonoBehaviour
{
    private TMP_InputField inputField;

    void Awake()
    {
        inputField = GetComponent<TMP_InputField>();
        inputField.onValueChanged.AddListener(OnTextChanged);
    }

    void OnTextChanged(string text)
    {
        if (text != text.ToUpper())
        {
            int caretPos = inputField.stringPosition;
            inputField.text = text.ToUpper();
            inputField.stringPosition = caretPos;
        }
    }
}
