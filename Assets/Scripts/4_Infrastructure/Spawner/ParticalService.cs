using System.Threading.Tasks;
using UnityEngine;

public class ParticalService
{
    private readonly IAdressablePoolService<GameObject> _poolService;
    private readonly ParticleLibrary _particleLibrary;

    public ParticalService(IAdressablePoolService<GameObject> poolService, ParticleLibrary particleLibrary)
    {
        _poolService = poolService;
        _particleLibrary = particleLibrary;
    }

    public async void Play(int id, Vector3 position)
    {
        var assetRef = _particleLibrary.Find(id);

        if(assetRef == null) { Debug.Log($"Not Found {id}"); return; }

        GameObject instance = await _poolService.AsyncGet(assetRef);
        instance.transform.position = position;
        instance.SetActive(true);
        var pSystem = instance.GetComponent<ParticleSystem>();
        pSystem.Play();

        ReturnAfterDelay(assetRef, instance, pSystem.main.duration);
    }

    public async void Play(int id, Vector3 position, Vector3 direction)
    {
        var assetRef = _particleLibrary.Find(id);
        if (assetRef == null)
        {
            Debug.LogWarning($"[ParticleService] Not Found: {id}");
            return;
        }

        GameObject instance = await _poolService.AsyncGet(assetRef);
        instance.transform.position = position;

        if (direction.sqrMagnitude > 0.0001f)
        {
            instance.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction.normalized);
        }
        else
        {
            instance.transform.rotation = Quaternion.identity;
        }

        instance.SetActive(true);

        var pSystem = instance.GetComponent<ParticleSystem>();
        if (pSystem == null)
        {
            Debug.LogWarning($"[ParticleService] {id} has no ParticleSystem component.");
            return;
        }

        pSystem.Play();
        ReturnAfterDelay(assetRef, instance, pSystem.main.duration);
    }
    private async void ReturnAfterDelay(GameObject assetReference, GameObject instance, float delay)
    {
        await Task.Delay((int)(delay * 1000));
        _poolService.Return(assetReference, instance);
    }
}
