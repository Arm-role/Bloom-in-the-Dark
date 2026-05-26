#nullable enable

using System;
using UnityEngine;

// Persistent bootstrap สำหรับ AudioService — สร้าง persistent GameObject (DontDestroyOnLoad)
// และ expose `Service` ให้ RuntimeInstaller bind เข้า DIContainerBase
// (MonoBehaviour triggers ที่ placed ในแต่ละ scene ใช้ `Service` ตรงๆ ได้)
public sealed class AudioBootstrap : MonoBehaviour
{
  public static IAudioService? Service { get; private set; }
  private static bool _isInitialized;

  [SerializeField] private ScriptableObject? _audioLibraryAsset;

  private void Awake()
  {
    if (_isInitialized)
    {
      Destroy(gameObject);
      return;
    }

    var library = _audioLibraryAsset as IAudioLibrary
      ?? throw new InvalidOperationException(
        $"[{nameof(AudioBootstrap)}] {nameof(_audioLibraryAsset)} ไม่ implement {nameof(IAudioLibrary)}");

    DontDestroyOnLoad(gameObject);
    _isInitialized = true;

    var audioGO = new GameObject("[AudioService]");
    DontDestroyOnLoad(audioGO);

    var service = audioGO.AddComponent<AudioService>();
    service.Initialize(library);

    Service = service;
  }
}
