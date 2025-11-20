using System;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private float hp;
    [SerializeField] private FlashHitView flashHitView;
    public void TakeDamage(float damage)
    {
        if (flashHitView != null)
        {
            flashHitView.FlashEffect();
        }

        hp -= damage;
    }
}
