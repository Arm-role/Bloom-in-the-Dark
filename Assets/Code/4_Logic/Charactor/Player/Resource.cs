using System;
using UnityEngine;


public enum ResourceChangeType
{
    Value,
    Max,
    Fill
}

public struct ResourceChangedEvent
{
    public float Current;
    public float Max;
    public ResourceChangeType ChangeType;
}

public class Resource
{
    public float Max { get; private set; }
    public float Current { get; private set; }

    public event Action<ResourceChangedEvent> OnChanged;
    public event Action<float> OnAmountRemoved;

    public Resource(float max)
    {
        Max = Mathf.Max(0f, max);
        Current = Max;
    }

    public float Percent => Max <= 0f ? 0f : Current / Max;

    public void Set(float value)
    {
        float clamped = Mathf.Clamp(value, 0f, Max);
        if (Mathf.Approximately(clamped, Current))
            return;

        Current = clamped;
        Raise(ResourceChangeType.Value);
    }

    public void SetMax(float value, bool refill = false)
    {
        float newMax = Mathf.Max(0f, value);
        if (Mathf.Approximately(newMax, Max))
            return;

        Max = newMax;
        Current = refill ? Max : Mathf.Clamp(Current, 0f, Max);
        Raise(ResourceChangeType.Max);
    }

    public void Add(float amount)
    {
        if (amount <= 0f) return;
        Set(Current + amount);
    }

    public bool CanRemove(float amount) => Current >= amount;

    public void Remove(float amount)
    {
        if (amount <= 0f) return;

        float before = Current;
        Set(Current - amount);

        float removed = before - Current;
        if (removed > 0f)
            OnAmountRemoved?.Invoke(removed);
    }

    public void AddMax(float amount, bool refill = false)
    {
        if (amount <= 0f) return;
        SetMax(Max + amount, refill);
    }

    public void Fill()
    {
        if (Mathf.Approximately(Current, Max)) return;
        Current = Max;
        Raise(ResourceChangeType.Fill);
    }
    public void ReFill()
    {
        float clamped = Mathf.Clamp(Max, 0f, Max);
        if (Mathf.Approximately(clamped, Current))
            return;

        Current = clamped;
        Raise(ResourceChangeType.Value);
    }

    private void Raise(ResourceChangeType type)
    {
        OnChanged?.Invoke(new ResourceChangedEvent
        {
            Current = Current,
            Max = Max,
            ChangeType = type
        });
    }

   
}