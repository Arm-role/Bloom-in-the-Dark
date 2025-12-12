using System;
using UnityEngine;

public class PlayerEnergy : IPlayerEnergy
{
    public float MaxEnergy { get; private set; }
    public float CurrentEnergy { get; private set; }

    public event Action<float, float> OnEnergyChanged;
    public event Action<float> OnAmmountRemoveChanged;

    public PlayerEnergy(float maxEnergy)
    {
        MaxEnergy = Mathf.Max(0, maxEnergy);
        CurrentEnergy = MaxEnergy;
    }

    // --- GET ---
    public float GetPercent() => MaxEnergy <= 0 ? 0 : CurrentEnergy / MaxEnergy;

    public bool IsFull => Mathf.Approximately(CurrentEnergy, MaxEnergy);
    public bool IsEmpty => CurrentEnergy <= 0f;

    // --- SET ---
    public void SetEnergy(float value)
    {
        CurrentEnergy = Mathf.Clamp(value, 0, MaxEnergy);
        RaiseEvent();
    }

    public void SetMaxEnergy(float value, bool refill = false)
    {
        MaxEnergy = Mathf.Max(0, value);
        CurrentEnergy = refill ? MaxEnergy : Mathf.Clamp(CurrentEnergy, 0, MaxEnergy);
        RaiseEvent();
    }

    // --- ADD / REMOVE ---
    public void Add(float amount)
    {
        if (amount <= 0) return;
        SetEnergy(CurrentEnergy + amount);
    }

    public void Remove(float amount)
    {
        if (amount <= 0) return;
        SetEnergy(CurrentEnergy - amount);
        Debug.Log(CurrentEnergy + " And " + amount);
        OnAmmountRemoveChanged?.Invoke(amount);
    }

    // --- ADD MAX / REMOVE MAX ---
    public void AddMax(float amount, bool refill = false)
    {
        if (amount <= 0) return;
        SetMaxEnergy(MaxEnergy + amount, refill);
    }

    public void RemoveMax(float amount)
    {
        if (amount <= 0) return;
        SetMaxEnergy(MaxEnergy - amount, false);
    }

    // --- FILL ---
    public void Fill() => SetEnergy(MaxEnergy);

    // --- CHECK ---
    public bool CanAdd(float amount) => CurrentEnergy < MaxEnergy && amount > 0;
    public bool CanRemove(float amount) => CurrentEnergy >= amount && amount > 0;

    private void RaiseEvent() => OnEnergyChanged?.Invoke(CurrentEnergy, MaxEnergy);
}