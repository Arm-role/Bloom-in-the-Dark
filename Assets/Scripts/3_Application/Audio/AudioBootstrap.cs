using UnityEngine;

public class AudioBootstrap : MonoBehaviour
{
  public static IAudioService Service { get; private set; }
  private static bool _isInitialized;

  [SerializeField] private ScriptableObject _audioLibraryAsset;

  private void Awake()
  {
    if (_isInitialized)
    {
      Destroy(gameObject);
      return;
    }

    var library = _audioLibraryAsset as IAudioLibrary;
    if (library == null)
    {
      Debug.LogError("[AudioBootstrap] _audioLibraryAsset does not implement IAudioLibrary");
      return;
    }

    DontDestroyOnLoad(gameObject);
    _isInitialized = true;

    var audioGO = new GameObject("[AudioService]");
    DontDestroyOnLoad(audioGO);

    var service = audioGO.AddComponent<AudioService>();
    service.Initialize(library);

    Service = service;
  }
}
