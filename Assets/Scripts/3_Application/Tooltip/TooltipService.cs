public interface ITooltipService
{
  void Show(TooltipData data);
  void Hide();
}

public class TooltipService : ITooltipService
{
  private readonly ITooltipView _view;
  public TooltipService(ITooltipView view) => _view = view;
  public void Show(TooltipData data) => _view.Show(data);
  public void Hide() => _view.Hide();
}