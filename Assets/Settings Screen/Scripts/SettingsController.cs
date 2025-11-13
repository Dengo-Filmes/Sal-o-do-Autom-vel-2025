using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] Image _indicatorLine;
    [SerializeField] Image _screenMask;

    CanvasGroup _currentSetting;
    CanvasGroup _previousSetting;

    bool _settingsOpen = false;

    [Header("Values")]
    float _brightnessValue;
    float p_brightnessValue = 0;

    float _contrastValue;
    float p_contrastValue = 0;

    float _saturationValue;
    float p_saturationValue = 0;

    float _volumeValue = 1;
    float p_volumeValue = 1;

    [Header("UI")]
    [SerializeField] Slider _brightnessSlider;
    [SerializeField] Slider _contrastSlider;
    [SerializeField] Slider _saturationSlider;
    [Space(10)]
    [SerializeField] Slider _volumeSlider;
    [Space(20)]
    [SerializeField] Slider _mapSlider;

    [Header("Settings")]
    [SerializeField] Volume _volume;
    [SerializeField] AudioMixer _mixer;

    Settings _settings;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadSettings();
    }

    // Update is called once per frame
    void Update()
    {
        ControlSliderValues();
    }

    #region Front-end
    public void OpenSettings(RectTransform button)
    {
        if (_indicatorLine.rectTransform.anchoredPosition.y == button.localPosition.y) return;

        _indicatorLine.rectTransform.anchoredPosition = new Vector2(0, button.localPosition.y);

        LeanTween.value(0, 1, 0.2f).setOnUpdate((float value) =>
        {
            _indicatorLine.fillAmount = value;
        }).setOnComplete(() =>
        {
            if (!_settingsOpen)
            {
                LeanTween.value(0, 1, 0.2f).setOnUpdate((float value2) =>
                {
                    _screenMask.fillAmount = value2;
                }).setOnComplete(() => _settingsOpen = true);
            }

            _currentSetting.alpha = 0;
            LeanTween.alphaCanvas(_currentSetting, 1, 0.2f).setOnStart(() => _currentSetting.interactable = true).setOnComplete(() =>
            {
                _currentSetting.blocksRaycasts = true;
                
            });
        });
    }

    public void SelectSettings(CanvasGroup canvas)
    {
        if (_currentSetting == canvas) return;

        if (_currentSetting)
        {
            _previousSetting = _currentSetting;

            _previousSetting.alpha = 0;
            _previousSetting.blocksRaycasts = false;
            _previousSetting.interactable = false;
        }

        _currentSetting = canvas;
    }

    public void ExitSettings()
    {
        LeanTween.value(0, 1, 0.2f).setOnStart(() =>
        {
            if (_currentSetting)
            {
                _currentSetting.alpha = 0;
                _currentSetting.blocksRaycasts = false;
                _currentSetting.interactable = false;
            }

            _screenMask.fillAmount = 0;
            _indicatorLine.fillAmount = 0;

            _indicatorLine.rectTransform.anchoredPosition = Vector2.zero;

            _settingsOpen = false;
        }).setDelay(0.2f);
    }

    public void ControlSliderValues()
    {
        TMP_Text brightnessText = _brightnessSlider.transform.parent.GetChild(2).GetComponent<TMP_Text>();
        TMP_Text contrastText = _contrastSlider.transform.parent.GetChild(2).GetComponent<TMP_Text>();
        TMP_Text saturationText = _saturationSlider.transform.parent.GetChild(2).GetComponent<TMP_Text>();
        TMP_Text volumeText = _volumeSlider.transform.parent.GetChild(2).GetComponent<TMP_Text>();
        TMP_Text mapText = _mapSlider.transform.parent.GetChild(2).GetComponent<TMP_Text>();

        brightnessText.text = _brightnessSlider.value.ToString("+#;-#;0");
        contrastText.text = _contrastSlider.value.ToString("+#;-#;0");
        saturationText.text = _saturationSlider.value.ToString("+#;-#;0");

        float textVolume = _volumeSlider.value * 100;
        volumeText.text = Mathf.RoundToInt(textVolume).ToString();
        mapText.text = _mapSlider.value.ToString();
        _mapSlider.maxValue = OpenSettingsController.Instance.totemPositions.Count;
    }
    #endregion

    // Graphics
    public void ChangeBrightness(float brightness)
    {
        if(_volume.profile.TryGet(out ColorAdjustments adjustments))
        {
            adjustments.postExposure.value = brightness / 10;
            _brightnessValue = brightness;
        }
    }

    public void ChangeContrast(float contrast)
    {
        if (_volume.profile.TryGet(out ColorAdjustments adjustments))
        {
            adjustments.contrast.value = contrast;
            _contrastValue = contrast;
        }
    }

    public void ChangeSaturation(float saturation)
    {
        if (_volume.profile.TryGet(out ColorAdjustments adjustments))
        {
            adjustments.saturation.value = saturation;
            _saturationValue = saturation;
        }
    }

    public void ChangeTotem(float totem)
    {
        if(OpenSettingsController.Instance.totemPositions.Count > 0)
        {
            OpenSettingsController.Instance.SetCurrentTotem(totem);
        }
    }

    // Sound
    public void ChangeVolume(float volume)
    {
        float value = volume / 100;
        value = Mathf.Clamp(value, 0.0001f, 100);

        _mixer.SetFloat("Volume", Mathf.Log10(volume) * 20);
        _volumeValue = volume;
    }

    // Save settings
    public void ApplySettings()
    {
        // Save settings
        _settings = new(_brightnessValue, _contrastValue, _saturationValue, _volumeValue);
        string json = JsonUtility.ToJson(_settings, true);

        string filePath = Path.Combine(Application.dataPath, "settings.ini");

        using(StreamWriter sw = new StreamWriter(filePath))
        {
            sw.Write(json);
        }
    }

    // Load settings
    void LoadSettings()
    {
        _volume = OpenSettingsController.Instance._volume;
        _mixer = OpenSettingsController.Instance._mixer;

        string filePath = Path.Combine(Application.dataPath, "settings.ini");

        if (File.Exists(filePath))
        {
            string json = string.Empty;
            using(StreamReader sr = new StreamReader(filePath))
            {
                json = sr.ReadToEnd();
            }

            Settings current = new(0, 0, 0, 0);
            JsonUtility.FromJsonOverwrite(json, current);
            _settings = current;

            ChangeBrightness(_settings.Brightness);
            ChangeContrast(_settings.Contrast);
            ChangeSaturation(_settings.Saturation);
            ChangeVolume(_settings.Volume);

            p_brightnessValue = _brightnessValue;
            p_contrastValue = _contrastValue;
            p_saturationValue = _saturationValue;
            p_volumeValue = _volumeValue;

            _brightnessSlider.value = p_brightnessValue;
            _contrastSlider.value = p_contrastValue;
            _saturationSlider.value = p_saturationValue;
            _volumeSlider.value = p_volumeValue;
        }
    }

    // Reset settings
    public void ResetDefaultValues()
    {
        WarningPanelController.Instance.CallWarning("Esta ação irá restaurar <b>todas</b> as configurações para o padrão. Confirmar?");
        WarningPanelController.Instance.GetConfirmButton.onClick.AddListener(() => ResetValues());
    }

    void ResetValues()
    {
        ChangeBrightness(0);
        ChangeContrast(0);
        ChangeSaturation(0);
        ChangeVolume(1);

        p_brightnessValue = 0;
        p_contrastValue = 0;
        p_saturationValue = 0;

        _brightnessSlider.value = 0;
        _contrastSlider.value = 0;
        _saturationSlider.value = 0;
        _volumeSlider.value = 1;
    }

    // Leave settings
    public void LeaveWithoutSaving()
    {
        // Ask if any value has changed
    }

    // Audio
}

[System.Serializable]
public class Settings
{
    public float Brightness = 0;
    public float Contrast = 0;
    public float Saturation = 0;
    public float Volume = 0;

    public Settings(float brightness, float contrast, float saturation, float volume)
    {
        Brightness = brightness;
        Contrast = contrast;
        Saturation = saturation;
        Volume = volume;
    }
}
