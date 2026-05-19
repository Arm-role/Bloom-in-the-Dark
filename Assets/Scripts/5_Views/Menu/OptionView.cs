using System;
using UnityEngine;
using UnityEngine.UI;

public class OptionView : MonoBehaviour, IOptionView
{
  [SerializeField] private GameObject _panel;
  [SerializeField] private Slider _musicSlider;
  [SerializeField] private Slider _sfxSlider;
  [SerializeField] private Button _closeButton;

  public event Action<float> OnMusicVolumeChanged;
  public event Action<float> OnSFXVolumeChanged;
  public event Action OnCloseClicked;

  private void Awake()
  {
    _musicSlider.onValueChanged.AddListener(v => OnMusicVolumeChanged?.Invoke(v));
    _sfxSlider.onValueChanged.AddListener(v => OnSFXVolumeChanged?.Invoke(v));
    _closeButton.onClick.AddListener(() => OnCloseClicked?.Invoke());
  }

  private void OnDestroy()
  {
    _musicSlider.onValueChanged.RemoveAllListeners();
    _sfxSlider.onValueChanged.RemoveAllListeners();
    _closeButton.onClick.RemoveAllListeners();
  }

  // SetValueWithoutNotify เพื่อไม่ให้ trigger event ตอน load PlayerPrefs
  public void SetMusicVolume(float value) => _musicSlider.SetValueWithoutNotify(value);
  public void SetSFXVolume(float value) => _sfxSlider.SetValueWithoutNotify(value);

  public void Show() => _panel.SetActive(true);
  public void Hide() => _panel.SetActive(false);
}
