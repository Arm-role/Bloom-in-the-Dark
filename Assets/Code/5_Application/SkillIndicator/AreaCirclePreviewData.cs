using UnityEngine;

public readonly struct AreaCirclePreviewData
{
    public readonly Vector2 Origin;
    public readonly Vector2 Center;
    public readonly Vector3 RangeScale;
    public readonly Vector3 AreaScale;

    public AreaCirclePreviewData(
        Vector2 origin,
        Vector2 center,
        Vector3 rangeScale,
        Vector3 areaScale)
    {
        Origin = origin;
        Center = center;
        RangeScale = rangeScale;
        AreaScale = areaScale;
    }
}