using UnityEngine;

public abstract class SceneInstaller : MonoBehaviour
{
    protected virtual void Start()
    {
        if (AppInstaller.IsReady)
        {
            Initialzed(AppInstaller.Container);
        }
        else
        {
            AppInstaller.OnServiceReady += Initialzed;
        }
    }

    protected virtual void OnDestroy()
    {
        AppInstaller.OnServiceReady -= Initialzed;
    }

    protected virtual void Initialzed(DIContainerBase container)
    {

    }
}
