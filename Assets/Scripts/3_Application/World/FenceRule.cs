using System;
using UnityEngine;

public class FenceRule
{
  // Bit layout: 0=North  1=East  2=South  3=West
  private static readonly Vector3Int[] Dirs =
  {
    Vector3Int.up,
    Vector3Int.right,
    Vector3Int.down,
    Vector3Int.left,
  };

  private readonly Vector3Int _cellPos;
  private readonly IFenceRuleSet _ruleSet;
  private readonly IFenceColliderRuleSet _colliderRuleSet;
  private readonly Func<Vector3Int, bool> _isFenceAt;
  private readonly Action<Sprite> _applySprite;
  private readonly Action<Vector2[]> _applyPath;

  public FenceRule(
    Vector3Int cellPos,
    IFenceRuleSet ruleSet,
    Func<Vector3Int, bool> isFenceAt,
    Action<Sprite> applySprite,
    IFenceColliderRuleSet colliderRuleSet = null,
    Action<Vector2[]> applyPath = null)
  {
    _cellPos = cellPos;
    _ruleSet = ruleSet;
    _isFenceAt = isFenceAt;
    _applySprite = applySprite;
    _colliderRuleSet = colliderRuleSet;
    _applyPath = applyPath;
  }

  public void UpdateBitmask()
  {
    if (_ruleSet == null) return;

    int mask = 0;
    for (int i = 0; i < Dirs.Length; i++)
    {
      if (_isFenceAt(_cellPos + Dirs[i]))
        mask |= (1 << i);
    }

    var sprite = _ruleSet.GetSprite(mask);
    if (sprite != null)
      _applySprite(sprite);

    if (_colliderRuleSet != null && _applyPath != null)
    {
      var path = _colliderRuleSet.GetPath(mask);
      if (path != null && path.Length > 0)
        _applyPath(path);
    }
  }
}
