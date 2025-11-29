using System;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable, IDestructible, IPoolable<GameObject>
{
    [SerializeField] private FlashHitView flashHitView;

    private float _hp = 30;

    public bool IsAlive { get; set; }

    public event Action<GameObject> OnRequestDestruction;

    public void OnSpawnFromPool(GameObject ob)
    {
        IsAlive = true;
        _hp = 30;
        flashHitView.SetObject();
    }
    public void OnReturnToPool(GameObject ob)
    {
        IsAlive = false;
    }

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

        if(_hp <= 0)
        {
            RequestDestruction();
        }
    }
}