using UnityEngine;

public class OptionController : MonoBehaviour
{
  private const string MUSIC_PREF = "MusicVolume";
  private const string SFX_PREF = "SFXVolume";
  private const float DEFAULT_VOLUME = 0.8f;

  [SerializeField] private ScriptableObject _audioLibraryAsset;
  [SerializeField] private MonoBehaviour _viewBehaviour;

  private IAudioLibrary _audioLibrary;
  private IOptionView _view;

  private void Start()
  {
    _audioLibrary = _audioLibraryAsset as IAudioLibrary;
    if (_audioLibrary == null)
    {
      Debug.LogWarning("[OptionController] _audioLibraryAsset does not implement IAudioLibrary");
      return;
    }

    _view = _viewBehaviour as IOptionView;
    if (_view == null)
    {
      Debug.LogWarning("[OptionController] _viewBehaviour does not implement IOptionView");
      return;
    }

    // load + apply ทันที (ไม่ trigger event เพราะใช้ SetValueWithoutNotify ใน View)
    float music = PlayerPrefs.GetFloat(MUSIC_PREF, DEFAULT_VOLUME);
    float sfx = PlayerPrefs.GetFloat(SFX_PREF, DEFAULT_VOLUME);

    ApplyMusicVolume(music);
    ApplySFXVolume(sfx);

    _view.SetMusicVolume(music);
    _view.SetSFXVolume(sfx);
    _view.Hide();

    _view.OnMusicVolumeChanged += HandleMusicVolume;
    _view.OnSFXVolumeChanged += HandleSFXVolume;
    _view.OnCloseClicked += Hide;
  }

  private void OnDestroy()
  {
    if (_view == null) return;
    _view.OnMusicVolumeChanged -= HandleMusicVolume;
    _view.OnSFXVolumeChanged -= HandleSFXVolume;
    _view.OnCloseClicked -= Hide;
  }

  public bool IsShown { get; private set; }

  public void Show()
  {
    IsShown = true;
    _view?.Show();
  }

  public void Hide()
  {
    IsShown = false;
    _view?.Hide();
  }

  private void HandleMusicVolume(float v)
  {
    ApplyMusicVolume(v);
    PlayerPrefs.SetFloat(MUSIC_PREF, v);
  }

  private void HandleSFXVolume(float v)
  {
    ApplySFXVolume(v);
    PlayerPrefs.SetFloat(SFX_PREF, v);
  }

  private void ApplyMusicVolume(float v) => _audioLibrary?.SetMusicVolume(v);
  private void ApplySFXVolume(float v) => _audioLibrary?.SetSFXVolume(v);
}
