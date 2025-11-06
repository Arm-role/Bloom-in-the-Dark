using UnityEngine;

public class AreaCircleSkillPreview : ISkillIndicatorPreview
{
    private AreaCircleIndicator _indicator;
    private AreaCirclePreview _view;
    private bool _isActive;

    public AreaCircleSkillPreview(AreaCircleIndicator indicator, AreaCirclePreview view)
    {
        _indicator = indicator;
        _view = view;
    }

    public void Setup(InteractionHandleContext context)
    {
        _view.Initialize();
        _view.Disable();
    }

    public void EnablePreview(InteractionHandleContext context)
    {
        _indicator.UpdatePlayerPosition(context.PlayerPosition.Value);
        _view.Enable();
        _isActive = true;
    }

    public void UpdatePreview(InteractionHandleContext context)
    {
        if (!_isActive) return;

        _indicator.UpdatePlayerPosition(context.PlayerPosition.Value);

        var (rangePos, healPos, rotation, rangeScale, healScale)
            = _indicator.CalculatePreview(context.PointerPosition.Value);

        _view.UpdateView(rangePos, healPos, rotation, rangeScale, healScale);
    }

    public void DisablePreview()
    {
        _view.Disable();
        _isActive = false;
    }
}