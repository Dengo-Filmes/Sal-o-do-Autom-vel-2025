using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;

public class IdleVideoController : MonoBehaviour, IPointerDownHandler
{
    [Header("Referências")]
    public VideoPlayer videoPlayer;
    public CanvasGroup videoGroup;
    public float fadeDuration = 0.5f;
    public float idleTime = 60f;

    private float timer;
    private bool videoActive = true;
    private bool fading = false;

    void Start()
    {
        if (videoPlayer)
        {
            videoPlayer.isLooping = true;
            videoPlayer.frame = 0;
            videoPlayer.Play();
        }

        if (videoGroup)
        {
            videoGroup.alpha = 1f;
            videoGroup.blocksRaycasts = true;
            videoGroup.interactable = true;
        }

        videoActive = true;
        timer = 0f;
    }

    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            ResetTimer();
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            ResetTimer();
        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
            ResetTimer();

        timer += Time.deltaTime;

        if (timer >= idleTime && !videoActive && !IsAnyVideoFullscreenActive())
        {
            ShowVideo();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ResetTimer();

        if (videoActive)
            HideVideo();
    }

    public void RegisterInteraction()
    {
        ResetTimer();

        if (videoActive)
            HideVideo();
    }

    private bool IsAnyVideoFullscreenActive()
    {
        VideoFullscreenToggle[] fullscreenPlayers = FindObjectsOfType<VideoFullscreenToggle>();
        foreach (VideoFullscreenToggle player in fullscreenPlayers)
        {
            if (player.fullscreenPanel != null && player.fullscreenPanel.activeInHierarchy)
            {
                return true;
            }
        }
        return false;
    }

    public void ForceCloseDropdowns()
    {
        TMP_Dropdown[] dropdowns = FindObjectsOfType<TMP_Dropdown>();
        foreach (TMP_Dropdown dropdown in dropdowns)
        {
            dropdown.Hide();

            if (dropdown.transform.childCount > 1)
            {
                Transform template = dropdown.transform.Find("Template");
                if (template != null && template.gameObject.activeInHierarchy)
                {
                    template.gameObject.SetActive(false);
                }
            }
        }
    }

    private void ResetTimer()
    {
        timer = 0f;
    }

    private void ShowVideo()
    {
        PathController.Instance.ResetPath();
        if (fading || videoActive) return;

        ForceCloseDropdowns();
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
            videoPlayer.Stop();
            videoPlayer.frame = 0;
            videoPlayer.Play();

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
            videoGroup.blocksRaycasts = false;
            videoGroup.interactable = false;
        }

        videoActive = show;
        fading = false;
        timer = 0f;
    }
}