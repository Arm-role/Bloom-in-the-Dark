using System;
using UnityEngine;

public class PlayerEnergy : IResource
{
  private readonly Resource resource;
  private readonly IStatDatabase _statDatabase;
  private readonly PhaseStatService _statService;

  private readonly StatKey _maxEnegyKey;
  private readonly StatKey _energyRefill;
  private readonly GameTag _ownerTag;

  public float Current => resource.Current;
  public float Max => resource.Max;

  public event Action<ResourceChangedEvent> OnChanged
  {
    add => resource.OnChanged += value;
    remove => resource.OnChanged -= value;
  }

  public PlayerEnergy(
    IStatDatabase statDatabase,
    PhaseStatService statService,
    GameTag ownerTag)
  {
    _statService = statService;
    _statDatabase = statDatabase;

    _maxEnegyKey = _statDatabase.MaxEnergy;
    _energyRefill = _statDatabase.EnergyRefill;
    _ownerTag = ownerTag;

    float maxEnegy = _statService.GetStat(_maxEnegyKey);
    resource = new Resource(maxEnegy);

    _statService.onUpgrade += OnStatChanged;

    Debug.Log(_statService.GetStat(_energyRefill));
  }

  private void OnStatChanged(GameTag tag, StatKey key)
  {
    if (tag.Hash != _ownerTag.Hash)
      return;

    if (key != _maxEnegyKey)
      return;

    float newMax = _statService.GetStat(_maxEnegyKey);
    resource.SetMax(newMax);
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

  public void ReFillAdd()
     => resource.Add(_statService.GetStat(_energyRefill));
}
