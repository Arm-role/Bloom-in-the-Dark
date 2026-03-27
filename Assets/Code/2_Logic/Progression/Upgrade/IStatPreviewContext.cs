public interface IStatPreviewContext
{
  float GetBefore(StatKey key);
  float GetAfter(StatModifier modifier);
}
