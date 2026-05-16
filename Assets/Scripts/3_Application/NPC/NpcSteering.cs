using UnityEngine;

// Steering แบบ simple สำหรับ NPC — อ่านทิศจาก FlowField โดยไม่มี separation/obstacle avoidance
[RequireComponent(typeof(FlowFieldOwner))]
public class NpcSteering : MonoBehaviour, IFlowKeyHolder
{
    public FlowFieldChannelKey FlowKey { get; set; }

    private FlowFieldOwner _owner;

    private void Awake()
    {
        _owner = GetComponent<FlowFieldOwner>();
    }

    public SteeringResult Sample()
    {
        var ff = FlowFieldManager.Instance;
        if (ff == null || !ff.TryGetField(FlowKey, _owner.Footprint, out var field))
            return SteeringResult.Zero;

        var grid = ff.world.GridConverter;
        var cell = grid.WorldToCell(transform.position);
        var local = new Vector2Int(cell.x - field.originCell.x, cell.y - field.originCell.y);

        if (!field.IsInside(local)) return SteeringResult.Zero;

        Vector2 dir = field.GetDirection(local);
        return dir == Vector2.zero
            ? SteeringResult.Zero
            : new SteeringResult(dir.normalized);
    }

    public bool HasDirection()
    {
        var ff = FlowFieldManager.Instance;
        if (ff == null || !ff.TryGetField(FlowKey, _owner.Footprint, out var field))
            return false;

        var grid = ff.world.GridConverter;
        var cell = grid.WorldToCell(transform.position);
        var local = new Vector2Int(cell.x - field.originCell.x, cell.y - field.originCell.y);

        return field.IsInside(local) && field.GetDirection(local) != Vector2.zero;
    }
}
