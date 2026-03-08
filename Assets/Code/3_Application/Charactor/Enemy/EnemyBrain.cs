public class EnemyBrain
{
  public EnemyStateType Think(EnemyBlackboard bb)
  {
    if (!bb.TargetVisible)
      return EnemyStateType.Idle;

    if (bb.TargetInAttackRange)
      return EnemyStateType.Attack;

    return EnemyStateType.Chase;
  }
}

public enum EnemyStateType
{
  Idle,
  Attack,
  Chase
}