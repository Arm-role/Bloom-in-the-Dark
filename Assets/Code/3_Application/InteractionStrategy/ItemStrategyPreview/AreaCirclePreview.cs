using UnityEngine;

public class AreaCirclePreview : IInteractionPreview
{
    private readonly AreaCircleShape _shape;
    private readonly IAreaCircleIndicatorPreview _view;

    public AreaCirclePreview(
        AreaCircleShape shape,
        IAreaCircleIndicatorPreview view)
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
        if (!result.IsValid || result.Extra is not Vector2 center)
        {
            _view.Disable();
            return;
        }

        Vector2 origin = result.Origin;

        var preview = _shape.GetPreview(origin, center);

        _view.Enable();
        _view.UpdateView(
            preview.Origin,
            preview.Center,
            preview.RangeScale,
            preview.AreaScale
        );
    }

    public void Hide()
    {
        _view.Disable();
    }
}