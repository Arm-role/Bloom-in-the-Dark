using UnityEngine;

public interface IAreaCircleIndicatorPreview 
{
    public void Initialize();
    public void UpdateView(
        Vector2 rangePos,
        Vector2 healPos,
        Vector3 rangeScale, 
        Vector3 healScale);
    
    public void Enable();
    public void Disable();
}