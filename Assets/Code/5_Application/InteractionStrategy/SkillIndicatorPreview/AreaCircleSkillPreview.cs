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
        Debug.Log("Setup Preview");
        _view.Initialize();
        _view.Disable();
    }

    public void EnablePreview(InteractionHandleContext context)
    {
        Debug.Log("EnablePreview Preview");

        _indicator.UpdatePlayerPosition(context.PlayerPosition.Value);
        _view.Enable();
        _isActive = true;
    }

    public void UpdatePreview(InteractionHandleContext context)
    {
        if (!_isActive) return;

        Debug.Log("UpdatePreview Preview");

        _indicator.UpdatePlayerPosition(context.PlayerPosition.Value);

        var (rangePos, healPos, rangeScale, healScale)
            = _indicator.CalculatePreview(context.PointerPosition.Value);

        _view.UpdateView(rangePos, healPos, rangeScale, healScale);
    }

    public void DisablePreview()
    {
        Debug.Log("DisablePreview Preview");

        _view.Disable();
        _isActive = false;
    }
}