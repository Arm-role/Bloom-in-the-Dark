using System;

public interface IStatService
{
  event Action<GameTag, StatKey> onUpgrade;
  float GetStat(StatKey key);
  float GetStatWithPreview(StatModifier previewModifier);
  StatBreakdown GetBreakdown(StatKey key);
}
