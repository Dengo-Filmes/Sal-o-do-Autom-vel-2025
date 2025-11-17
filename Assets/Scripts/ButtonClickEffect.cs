using UnityEngine;
using UnityEngine.UI;

public class ButtonClickEffect : MonoBehaviour
{
    [Header("Configurações do efeito")]
    public float scaleAmount = 1.2f;    // quanto aumenta ao clicar
    public float duration = 0.2f;       // duração do efeito
    private Vector3 originalScale;

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        if (button == null)
        {
            Debug.LogWarning("Este script precisa estar em um GameObject com Button!");
            enabled = false;
            return;
        }

        originalScale = transform.localScale;

        // adiciona o listener do clique
        button.onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        // anima o botão para crescer e voltar
        LeanTween.scale(gameObject, originalScale * scaleAmount, duration)
                 .setEase(LeanTweenType.easeOutQuad)
                 .setOnComplete(() =>
                 {
                     LeanTween.scale(gameObject, originalScale, duration).setEase(LeanTweenType.easeInQuad);
                 });
    }

    private void OnDestroy()
    {
        // remove listener para evitar erros
        if (button != null)
            button.onClick.RemoveListener(OnButtonClick);
    }
}
