using UnityEngine;

public class AttackState : IEnemyState
{
  private EnemyController _c;
  private EnemyPatternBrain _brain;

  public AttackState(EnemyController c)
  {
    _c = c;
    _brain = c.GetComponent<EnemyPatternBrain>();
  }

  public void Enter()
  {
    //_c.Locomotion.StopMovement();
    Debug.Log($"[AttackState] Enter — brain={_brain}, target={_c.CurrentTarget}");

    _brain?.StopPattern();

    var target = _c.CurrentTarget;
    if (target != null)
      _brain?.Tick(_c, target);
  }

  public void ManualUpdate()
  {
    if (!_c.Health.IsAlive)
    {
      _c.ChangeState(_c.DeadState);
      return;
    }

    if (_c.CurrentTarget == null)
    {
      _c.ChangeState(_c.ChaseState);
      return;
    }

    float dist = Vector2.Distance(
        _c.transform.position,
        _c.CurrentTarget.position);

    Debug.Log($"[AttackState] dist={dist:F2} AnyReady={_c.Combat.AnySkillReadyInRange(dist)}");

    // ✅ เพิ่ม — ถ้า pattern จบแล้วและออกนอก range ให้กลับ Chase
    if (_brain != null &&
        !_brain.IsRunning &&   // ← ต้องเพิ่ม property นี้
        !_c.Combat.AnySkillReadyInRange(dist))
    {
      _c.ChangeState(_c.ChaseState);
    }
  }
  public void Exit()
  {
    _brain?.StopPattern();
  }
  public void ManualFixedUpdate() { }
}