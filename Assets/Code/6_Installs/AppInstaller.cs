using System;
using UnityEngine;

public class AppInstaller : MonoBehaviour
{
    public static DIContainerBase Container;
    private static bool _isInitialzed = false;

    public static bool IsReady { get; private set; } = false;
    public static event Action<DIContainerBase> OnServiceReady;
    private void Awake()
    {
        if (_isInitialzed)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        _isInitialzed = true;

        Container = new DIContainerBase();

        IAdressablePoolService<GameObject> poolService = new AdressablePoolingService();

        Container.Register(poolService);

        var bootstrap = FindObjectOfType<GameBootstrap>();
        bootstrap.Initialize(Container);

        IsReady = true;
        OnServiceReady?.Invoke(Container);
    }
}
