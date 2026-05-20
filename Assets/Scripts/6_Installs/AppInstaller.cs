using System;
using UnityEngine;

public class AppInstaller : MonoBehaviour
{
  public static DIContainerBase Container;
  private static bool _isInitialized = false;

  public static bool IsReady { get; private set; } = false;
  public static event Action<DIContainerBase> OnServiceReady;
  private void Awake()
  {
    if (_isInitialized)
    {
      Destroy(gameObject);
      return;
    }

    DontDestroyOnLoad(gameObject);
    _isInitialized = true;

    Container = new DIContainerBase();

    IAdressablePoolService<GameObject> poolService = new AdressablePoolingService();

    Container.Register(poolService);

    var bootstrap = FindObjectOfType<GameBootstrap>();
    if (bootstrap == null)
    {
      Debug.LogError("[AppInstaller] GameBootstrap not found in boot scene — app cannot start");
      return; // IsReady คง false — log บอกชัด แทนที่จะ NRE เงียบ
    }
    bootstrap.Initialize(Container);

    Container.Register(bootstrap);

    IsReady = true;
    OnServiceReady?.Invoke(Container);
  }
}
