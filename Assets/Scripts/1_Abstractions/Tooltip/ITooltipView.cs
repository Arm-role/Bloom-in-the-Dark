public interface ITooltipView
{
  void Show(TooltipData data);
  void Hide();
}
public readonly struct TooltipData
{
  public readonly string Title;
  public readonly string Description;
  public readonly (string label, float value)[] Stats;

  public TooltipData(string title, string description,
                     (string, float)[] stats = null)
  {
    Title = title;
    Description = description;
    Stats = stats ?? System.Array.Empty<(string, float)>();
  }
}