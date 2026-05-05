public class ConePreview : IInteractionPreview
{
  private readonly ConeShape _shape;
  private readonly IConeIndicatorPreview _view;

  public ConePreview(ConeShape shape, IConeIndicatorPreview view)
  {
    _shape = shape;
    _view = view;
  }

  public void Setup()
  {
    _view.Initialize();
    _view.Disable();
  }

  public void Update(TargetResult result)
  {
    if (!result.IsValid)
    {
      _view.Disable();
      return;
    }

    var data = _shape.GetPreview(result.Origin, result.Origin + result.Direction);

    _view.Enable();
    _view.UpdateView(
        data.Origin,
        data.Direction,
        data.RangeScale,
        data.ConeScale,
        data.Angle);
  }

  public void Hide() => _view.Disable();
}