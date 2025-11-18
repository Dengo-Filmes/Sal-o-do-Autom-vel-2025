using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;
using System.Collections.Generic;

public class IdleVideoWithTransition : MonoBehaviour, IPointerDownHandler
{
    [Header("Video Standby Settings")]
    public VideoPlayer videoPlayer;
    public CanvasGroup videoGroup;
    public float fadeDuration = 0.5f;
    public float idleTime = 60f;

    [Header("Transition Settings")]
    public List<Sprite> transitionSprites;
    public Image displayImage;
    public float framesPerSecond = 60f;

    private float timer;
    private bool videoActive = true;
    private bool inTransition = false;

    void Start()
    {
        InitializeVideo();
    }

    void InitializeVideo()
    {
        if (videoPlayer != null)
        {
            videoPlayer.isLooping = true;
            videoPlayer.frame = 0;
            videoPlayer.Play();
        }

        if (videoGroup != null)
        {
            videoGroup.alpha = 1f;
            videoGroup.blocksRaycasts = true;
            videoGroup.interactable = true;
        }

        if (displayImage != null)
        {
            displayImage.gameObject.SetActive(false);
        }

        videoActive = true;
        timer = 0f;
        inTransition = false;
    }

    void Update()
    {
        HandleInput();
        HandleIdleTimer();
    }

    void HandleInput()
    {
        if (inTransition) return;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            OnUserInteraction();
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            OnUserInteraction();
        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
            OnUserInteraction();
    }

    void HandleIdleTimer()
    {
        if (!videoActive && !inTransition)
        {
            timer += Time.deltaTime;
            if (timer >= idleTime && !IsAnyVideoFullscreenActive())
            {
                ShowVideo();
            }
        }
    }

    void OnUserInteraction()
    {
        if (inTransition) return;

        ResetTimer();

        if (videoActive)
        {
            StartVideoTransition();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnUserInteraction();
    }

    public void RegisterInteraction()
    {
        OnUserInteraction();
    }

    void StartVideoTransition()
    {
        if (inTransition || !videoActive) return;

        if (HasTransitionSprites())
        {
            StartCoroutine(PlayTransitionSequence());
        }
        else
        {
            StartCoroutine(HideVideoWithFade());
        }
    }

    IEnumerator PlayTransitionSequence()
    {
        inTransition = true;

        if (displayImage != null)
        {
            displayImage.gameObject.SetActive(true);
            yield return StartCoroutine(AnimateSprites());
            displayImage.gameObject.SetActive(false);
        }

        HideVideoComplete();
        StartGame();

        inTransition = false;
    }

    IEnumerator AnimateSprites()
    {
        if (transitionSprites.Count == 0 || displayImage == null) yield break;

        float frameTime = 1f / framesPerSecond;
        float totalAnimationTime = transitionSprites.Count * frameTime;
        float timeToStopVideo = totalAnimationTime - 0.5f; // segundos antes do final

        for (int i = 0; i < transitionSprites.Count; i++)
        {
            displayImage.sprite = transitionSprites[i];

            float currentTime = i * frameTime;

            if (currentTime >= timeToStopVideo && videoPlayer != null)
            {
                videoPlayer.Stop();
                if (videoGroup != null)
                {
                    videoGroup.alpha = 0f;
                }
            }

            yield return new WaitForSeconds(frameTime);
        }

        displayImage.sprite = transitionSprites[transitionSprites.Count - 1];
    }

    IEnumerator HideVideoWithFade()
    {
        inTransition = true;

        float startAlpha = videoGroup.alpha;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            videoGroup.alpha = Mathf.Lerp(startAlpha, 0f, t);
            yield return null;
        }

        videoGroup.alpha = 0f;
        HideVideoComplete();
        StartGame();

        inTransition = false;
    }

    void HideVideoComplete()
    {
        if (videoPlayer != null)
            videoPlayer.Stop();

        if (videoGroup != null)
        {
            videoGroup.alpha = 0f;
            videoGroup.blocksRaycasts = false;
            videoGroup.interactable = false;
        }

        videoActive = false;
        ResetTimer();
    }

    void StartGame()
    {
        if (PathController.Instance != null)
            PathController.Instance.ResetPath();

        ForceCloseDropdowns();
    }

    void ShowVideo()
    {
        if (inTransition || videoActive) return;

        if (PathController.Instance != null)
            PathController.Instance.ResetPath();

        ForceCloseDropdowns();
        StartCoroutine(ShowVideoWithFade());
    }

    IEnumerator ShowVideoWithFade()
    {
        inTransition = true;

        if (videoPlayer != null)
        {
            videoPlayer.Stop();
            videoPlayer.frame = 0;
            videoPlayer.Play();
        }

        float startAlpha = videoGroup != null ? videoGroup.alpha : 0f;
        float t = 0f;

        while (t < 1f && videoGroup != null)
        {
            t += Time.deltaTime / fadeDuration;
            videoGroup.alpha = Mathf.Lerp(startAlpha, 1f, t);
            yield return null;
        }

        if (videoGroup != null)
        {
            videoGroup.alpha = 1f;
            videoGroup.blocksRaycasts = true;
            videoGroup.interactable = true;
        }

        videoActive = true;
        inTransition = false;
        ResetTimer();
    }

    bool HasTransitionSprites()
    {
        return transitionSprites != null && transitionSprites.Count > 0 && displayImage != null;
    }

    bool IsAnyVideoFullscreenActive()
    {
        VideoFullscreenToggle[] fullscreenPlayers = FindObjectsOfType<VideoFullscreenToggle>();
        if (fullscreenPlayers == null) return false;

        foreach (VideoFullscreenToggle player in fullscreenPlayers)
        {
            if (player != null && player.fullscreenPanel != null && player.fullscreenPanel.activeInHierarchy)
            {
                return true;
            }
        }
        return false;
    }

    public void ForceCloseDropdowns()
    {
        TMP_Dropdown[] dropdowns = FindObjectsOfType<TMP_Dropdown>();
        if (dropdowns == null) return;

        foreach (TMP_Dropdown dropdown in dropdowns)
        {
            if (dropdown != null)
            {
                dropdown.Hide();

                Transform template = dropdown.transform.Find("Template");
                if (template != null && template.gameObject.activeInHierarchy)
                {
                    template.gameObject.SetActive(false);
                }
            }
        }
    }

    void ResetTimer()
    {
        timer = 0f;
    }
}