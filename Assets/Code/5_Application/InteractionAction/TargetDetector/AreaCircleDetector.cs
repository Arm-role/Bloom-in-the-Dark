using UnityEngine;

public class AreaCircleDetector : ITargetDetector
{
    private readonly AreaCircleIndicator _processor;

    public AreaCircleDetector(AreaCircleIndicator processor)
    {
        _processor = processor;
    }

    public void Setup(InteractionHandleContext context)
    {
        if (context.ItemInstance.ItemData is not PlantItem item) return;

        _processor.Setup(item.Range, item.AreaRadius);
        _processor.UpdatePlayerPosition(context.PlayerPosition.Value);
    }

    public IDataProvider IntercationExcute(InteractionHandleContext context)
    {
        var (playerPos, areaRediusPos, range, areaRadius, yScale) = _processor.GetEllipseData();

        var pointerPosition = new Vector2(areaRediusPos.x, areaRediusPos.y);

        PlacementState state = CheckPlacement(areaRediusPos) ? PlacementState.Valid : PlacementState.Blocked;

        return new AreaCircleData(context.ItemInstance, pointerPosition, state);
    }
    private bool CheckPlacement(Vector2 healPos)
    {
        Collider2D hit = Physics2D.OverlapCircle(healPos, 0.25f, LayerMask.GetMask("Obstacle"));
        if (hit != null)
            return false;

        if (Physics.Raycast(new Vector3(healPos.x, 10f, healPos.y), Vector3.down, out RaycastHit hit3D, 20f))
        {
            if (hit3D.collider.CompareTag("Ground"))
                return true;
        }

        return true;
    }
}
