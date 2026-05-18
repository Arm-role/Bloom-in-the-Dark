public class RunStatPreviewContext : IStatPreviewContext
{
  private readonly IStatService _statService;
  public RunStatPreviewContext(IStatService statService)
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