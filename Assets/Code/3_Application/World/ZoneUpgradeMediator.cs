public class ZoneUpgradeMediator
{
  private readonly PhaseStatService _phaseStatService;
  private readonly WorldZoneManager _zoneManager;

  private readonly IVFXService _vFXService;
  private readonly StatKey farmAreaKey;

  public ZoneUpgradeMediator(
    IStatDatabase statDatabase,
    TagUpgradeThresholdService thresholdService,
    PhaseStatService phaseStatService,
    WorldZoneManager zoneManager,
    IVFXService vFXService)
  {
    _phaseStatService = phaseStatService;
    _zoneManager = zoneManager;
    _vFXService = vFXService;

    farmAreaKey = statDatabase.FarmArea;

    thresholdService.OnThresholdReward += OnThresholdReached;
  }

  private void OnThresholdReached(UpgradeData upgrade)
  {
    foreach (var mod in upgrade.modifiers)
    {
      if (farmAreaKey.RuntimeTag.Hash == mod.StatKey.RuntimeTag.Hash)
      {
        float radius = _phaseStatService.GetStat(mod.StatKey);
        _zoneManager.ZoneChange(
          radius,
          CellZoneFlags.SafeZone
        );

        _vFXService.ApplyFog(radius);
        return;
      }
    }
  }
}
