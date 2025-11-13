using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class IdleVideoController : MonoBehaviour, IPointerDownHandler
{
    [Header("Referências")]
    public VideoPlayer videoPlayer;
    public CanvasGroup videoGroup; // CanvasGroup do vídeo
    public float fadeDuration = 0.5f;
    public float idleTime = 60f; // tempo sem interação

    private float timer;
    private bool videoActive = true;
    private bool fading = false;

    void Start()
    {
        // Garante que o vídeo inicia corretamente
        if (videoPlayer)
        {
            videoPlayer.isLooping = true;
            videoPlayer.frame = 0;
            videoPlayer.Play();
        }

        if (videoGroup)
        {
            videoGroup.alpha = 1f;
            videoGroup.blocksRaycasts = true; // bloqueia toque enquanto visível
            videoGroup.interactable = true;
        }

        videoActive = true;
        timer = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= idleTime && !videoActive)
        {
            ShowVideo();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        timer = 0f; // reseta o contador sempre que clicar

        if (videoActive)
        {
            HideVideo();
        }
    }

    public void RegisterInteraction()
    {
        timer = 0f;
        if (videoActive)
            HideVideo();
    }

    private void ShowVideo()
    {
        if (fading || videoActive) return;
        StartCoroutine(FadeVideo(true));
    }

    private void HideVideo()
    {
        if (fading || !videoActive) return;
        StartCoroutine(FadeVideo(false));
    }

    private IEnumerator FadeVideo(bool show)
    {
        fading = true;

        float startAlpha = videoGroup.alpha;
        float endAlpha = show ? 1f : 0f;
        float t = 0f;

        if (show)
        {
            // Reinicia completamente o vídeo para evitar frame travado
            videoPlayer.Stop();
            videoPlayer.frame = 0;
            videoPlayer.Play();

            // Ativa bloqueio de toque
            videoGroup.blocksRaycasts = true;
            videoGroup.interactable = true;
        }

        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            videoGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            yield return null;
        }

        videoGroup.alpha = endAlpha;

        if (!show)
        {
            videoPlayer.Stop();

            // Libera os cliques no mapa
            videoGroup.blocksRaycasts = false;
            videoGroup.interactable = false;
        }

        videoActive = show;
        fading = false;
        timer = 0f;
    }
}
