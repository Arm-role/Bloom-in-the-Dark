using System;

public class PlayerHealth : IResource
{
  private readonly Resource resource;

  public float Current => resource.Current;
  public float Max => resource.Max;
  public bool IsAlive => Current > 0;

  public event Action<ResourceChangedEvent> OnChanged
  {
    add => resource.OnChanged += value;
    remove => resource.OnChanged -= value;
  }

  public PlayerHealth(float maxHP)
  {
    resource = new Resource(maxHP);
  }

  public void TakeDamage(float amount)
        => resource.Remove(amount);

  public void Heal(float amount)
      => resource.Add(amount);

  public void SetMax(float amount)
      => resource.SetMax(amount);

  public void Fill()
      => resource.Fill();
}