using System;

public class PlayerHealth : IResource
{
  private readonly Resource resource;
  private readonly IStatDatabase _statDatabase;
  private readonly PhaseStatService _statService;

  private readonly StatKey _maxHpKey;
  private readonly StatKey _hpRefill;
  private readonly GameTag _ownerTag;

  public float Current => resource.Current;
  public float Max => resource.Max;
  public bool IsAlive => Current > 0;


  public event Action<ResourceChangedEvent> OnChanged
  {
    add => resource.OnChanged += value;
    remove => resource.OnChanged -= value;
  }

  public PlayerHealth(
   IStatDatabase statDatabase,
   PhaseStatService statService,
   GameTag ownerTag)
  {
    _statService = statService;
    _statDatabase = statDatabase;

    _maxHpKey = _statDatabase.MaxHp;
    _hpRefill = _statDatabase.HpRefill;
    _ownerTag = ownerTag;

    float maxHp = _statService.GetStat(_maxHpKey);
    resource = new Resource(maxHp);

    _statService.onUpgrade += OnStatChanged;
  }
  private void OnStatChanged(GameTag tag, StatKey key)
  {
    if (tag.Hash != _ownerTag.Hash)
      return;

    if (key != _maxHpKey)
      return;

    float newMax = _statService.GetStat(_maxHpKey);
    resource.SetMax(newMax);
  }

  public void TakeDamage(float amount)
        => resource.Remove(amount);

  public void Heal(float amount)
      => resource.Add(amount);

  public void SetMax(float amount)
      => resource.SetMax(amount);

  public void Fill()
      => resource.Fill();
  public void ReFillAdd()
   => resource.Add(_statService.GetStat(_hpRefill));
}