using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[System.Serializable]
public class VideoEntry
{
    public string videoName;
    public VideoClip clip;
}

public class VideoDropdownController : MonoBehaviour
{
    [Header("UI")]
    public TMP_Dropdown dropdown;
    public RawImage videoDisplay;

    [Header("Video")]
    public VideoPlayer videoPlayer;

    [Header("Lista de vídeos")]
    public List<VideoEntry> videos = new List<VideoEntry>();

    void Start()
    {
        if (dropdown == null || videoPlayer == null || videoDisplay == null)
        {
            Debug.LogError("Dropdown, VideoPlayer ou RawImage não configurados!");
            return;
        }

        dropdown.ClearOptions();
        List<string> names = new List<string>();
        foreach (var v in videos) names.Add(v.videoName);

        dropdown.AddOptions(names);
        dropdown.onValueChanged.AddListener(OnDropdownSelect);

        if (videos.Count > 0)
            PlayVideo(0);
    }

    void OnDropdownSelect(int index)
    {
        PlayVideo(index);
    }

    void PlayVideo(int index)
    {
        if (index < 0 || index >= videos.Count) return;

        videoPlayer.clip = videos[index].clip;
        videoPlayer.isLooping = true;
        videoPlayer.Play();
    }                               
}
