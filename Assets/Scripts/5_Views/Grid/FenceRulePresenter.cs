using System;
using UnityEngine;

public class FenceRulePresenter : MonoBehaviour, IFenceUpdatable
{
  [SerializeField] private SpriteRenderer _renderer;
  [SerializeField] private FenceRuleSet _ruleSetAsset;
  [SerializeField] private FenceColliderRuleSet _colliderRuleSetAsset;
  [SerializeField] private PolygonCollider2D _collider;

  private FenceRule _rule;

  public void Initialize(Vector3Int cellPos, Func<Vector3Int, bool> isFenceAt)
  {
    _rule = new FenceRule(
      cellPos,
      _ruleSetAsset,
      isFenceAt,
      sprite => _renderer.sprite = sprite,
      _colliderRuleSetAsset,
      _collider != null ? path => _collider.SetPath(0, path) : null
    );
    _rule.UpdateBitmask();
  }

  public void UpdateBitmask() => _rule?.UpdateBitmask();
}
