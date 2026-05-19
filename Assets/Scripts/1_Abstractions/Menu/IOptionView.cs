using System;

public interface IOptionView
{
  event Action<float> OnMusicVolumeChanged;
  event Action<float> OnSFXVolumeChanged;
  event Action OnCloseClicked;

  void SetMusicVolume(float value);
  void SetSFXVolume(float value);
  void Show();
  void Hide();
}
