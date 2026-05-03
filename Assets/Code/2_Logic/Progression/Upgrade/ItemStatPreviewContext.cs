public class ItemStatPreviewContext : IStatPreviewContext
{
  private readonly ItemStatService _statService;

  public ItemStatPreviewContext(ItemStatService statService)
  {
    _statService = statService;
  }

  public float GetBefore(StatKey key)
  {
    return _statService.GetStat(key);
  }

  public float GetAfter(StatModifier modifier)
  {
    return _statService.GetStatWithPreview(modifier);
  }
}
