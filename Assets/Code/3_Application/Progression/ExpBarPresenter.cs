
public class ExpBarPresenter
{
  private readonly PlayerProgression _progression;
  private readonly IBarView _barView;

  public ExpBarPresenter(PlayerProgression progression, IBarView barView)
  {
    _progression = progression;
    _barView = barView;

    _progression.OnExpChanged += UpdateBar;
    _progression.Setup();
  }

  private void UpdateBar(float current, float max)
  {
    _barView.SetHealth(current, max);
  }

  public void Dispose()
  {
    _progression.OnExpChanged -= UpdateBar;
  }
}
