using System;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable, IDestructible, IPoolable<GameObject>
{
    private float _hp = 30;

    private IFlashHitView flashHitView;
    public bool IsAlive { get; set; }

    public event Action<GameObject> OnRequestDestruction;

    public void OnSpawnFromPool(GameObject ob)
    {
        _hp = 30;

        if(flashHitView == null)
            flashHitView = GetComponent<IFlashHitView>();

        flashHitView.SetObject();
    }
    public void OnReturnToPool(GameObject ob) { }


    public void RequestDestruction()
    {
        OnRequestDestruction?.Invoke(gameObject);
    }

    public void TakeDamage(float damage)
    {
        if (flashHitView != null)
        {
            flashHitView.FlashEffect();
        }

        _hp -= damage;

        if (_hp <= 0)
        {
            RequestDestruction();
        }
    }
}
