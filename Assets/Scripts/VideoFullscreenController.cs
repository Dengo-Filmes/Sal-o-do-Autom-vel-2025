using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

[RequireComponent(typeof(Button))]
public class VideoFullscreenToggle : MonoBehaviour, IPointerClickHandler
{
    [Header("Main (dropdown)")]
    public VideoPlayer mainVideoPlayer;     
    public RawImage mainRawImage;           

    [Header("Fullscreen (preparado)")]
    public GameObject fullscreenPanel;      
    public RawImage fullscreenRawImage;     
    public VideoPlayer fullscreenVideoPlayer; 

    [Header("Config")]
    public bool pauseMainWhileFullscreen = true;

    bool isFullscreen = false;

    void Start()
    {
        if (fullscreenPanel != null)
            fullscreenPanel.SetActive(false);

        if (fullscreenVideoPlayer != null && fullscreenRawImage != null)
        {
            if (fullscreenVideoPlayer.targetTexture != null)
                fullscreenRawImage.texture = fullscreenVideoPlayer.targetTexture;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isFullscreen) EnterFullscreen();
    }

    public void EnterFullscreen()
    {
        if (mainVideoPlayer == null || fullscreenVideoPlayer == null || fullscreenRawImage == null || fullscreenPanel == null)
        {
            Debug.LogWarning("VideoFullscreenToggle: atribua mainVideoPlayer, fullscreenVideoPlayer, fullscreenRawImage e fullscreenPanel no inspector.");
            return;
        }

        fullscreenVideoPlayer.Stop();
        fullscreenVideoPlayer.clip = mainVideoPlayer.clip;
        fullscreenVideoPlayer.time = mainVideoPlayer.time;
        fullscreenVideoPlayer.playOnAwake = false;
        fullscreenVideoPlayer.isLooping = mainVideoPlayer.isLooping;

        fullscreenPanel.SetActive(true);

        if (fullscreenVideoPlayer.targetTexture != null)
            fullscreenRawImage.texture = fullscreenVideoPlayer.targetTexture;

        if (pauseMainWhileFullscreen)
            mainVideoPlayer.Pause();

        fullscreenVideoPlayer.Play();

        var catcher = fullscreenPanel.GetComponent<FullscreenClickCatcher>();
        if (catcher == null) catcher = fullscreenPanel.AddComponent<FullscreenClickCatcher>();
        catcher.controller = this;

        isFullscreen = true;
    }

    public void ExitFullscreen()
    {
        if (!isFullscreen) return;

        if (fullscreenVideoPlayer.isPlaying)
            fullscreenVideoPlayer.Pause();

        double t = fullscreenVideoPlayer.time;
        if (mainVideoPlayer.clip == fullscreenVideoPlayer.clip)
        {
            mainVideoPlayer.time = t;
            if (pauseMainWhileFullscreen == false)
            {
            }
            else
            {
                mainVideoPlayer.Play();
            }
        }

        fullscreenPanel.SetActive(false);

        var catcher = fullscreenPanel.GetComponent<FullscreenClickCatcher>();
        if (catcher != null) Destroy(catcher);

        isFullscreen = false;
    }
}

public class FullscreenClickCatcher : MonoBehaviour, IPointerClickHandler
{
    public VideoFullscreenToggle controller;
    public void OnPointerClick(PointerEventData eventData)
    {
        controller?.ExitFullscreen();
    }
}
