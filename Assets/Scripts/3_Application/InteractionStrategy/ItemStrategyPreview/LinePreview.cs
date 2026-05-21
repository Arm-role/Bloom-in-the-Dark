using UnityEngine;

public class LinePreview : IInteractionPreview
{
  private readonly AreaLineShape _shape;
  private readonly IAreaLineIndicatorPreview _view;

  public LinePreview(AreaLineShape shape, IAreaLineIndicatorPreview view)
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

    // _shape ถูก Setup() แล้วจาก AreaLineTargetStrategy.Resolve ที่รันก่อนหน้า → Length ใช้ได้
    Vector2 end = result.Origin + result.Direction * _shape.Length;
    var data = _shape.GetPreview(result.Origin, end);

    _view.Enable();
    _view.UpdateView(data.Origin, data.Scale, data.Angle);
  }

  public void Hide() => _view.Disable();
}
