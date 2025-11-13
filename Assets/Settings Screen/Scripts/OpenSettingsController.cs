using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class OpenSettingsController : MonoBehaviour
{
    public static OpenSettingsController Instance;

    public GameObject root;

    public Volume _volume;
    public AudioMixer _mixer;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenSettings()
    {
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
        root.SetActive(false);
    }


}
