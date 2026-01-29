using System;
using UnityEngine;

public class PlayerEnergy : IResource
{
    private readonly Resource resource;

    public float Current => resource.Current;
    public float Max => resource.Max;

    public event Action<ResourceChangedEvent> OnChanged
    {
        add => resource.OnChanged += value;
        remove => resource.OnChanged -= value;
    }

    public PlayerEnergy(float maxEnergy)
    {
        resource = new Resource(maxEnergy);
    }

    public bool CanRemove(float amount)
        => resource.CanRemove(amount);

    public void Remove(float amount)
        => resource.Remove(amount);

    public void Add(float amount)
        => resource.Add(amount);

    public void SetMax(float amount)
        => resource.SetMax(amount);

    public void AddMax(float amount)
        => resource.AddMax(amount);

    public void Fill()
        => resource.Fill();
    
    public void ReFill()
        => resource.ReFill();
}