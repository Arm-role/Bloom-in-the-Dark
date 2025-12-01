using UnityEngine;

public class AttackState : IEnemyState
{
    private EnemyController _c;

    private float _attackTimer = 0f;
    private float _stateTimer = 0f;

    private IEnemySkill _currentSkill;
    private bool _isExecuting = false;

    public AttackState(EnemyController c)
    {
        _c = c;
    }

    public void Enter()
    {
        _stateTimer = 0f;
        _attackTimer = 0f;
        _isExecuting = false;
        _currentSkill = null;

        _c.Movement.Stop();
    }

    public void Exit()
    {
        _isExecuting = false;
        _currentSkill = null;
    }

    public void ManualUpdate()
    {
        if (_c.Data.IsDead)
        {
            _c.ChangeState(_c.DeadState);
            return;
        }

        var player = _c.Player;
        if (player == null) return;

        float dist = Vector2.Distance(_c.transform.position, player.position);

        // out of attack range → return to chase
        if (!_isExecuting && !_c.Combat.AnySkillReadyInRange(dist))
        {
            _c.ChangeState(_c.ChaseState);
            return;
        }

        // if not executing a skill → pick one
        if (!_isExecuting)
        {
            _currentSkill = _c.Combat.GetBestSkill(dist);
            if (_currentSkill != null)
            {
                StartSkill(player);
            }
        }

        _stateTimer += Time.deltaTime;
    }

    public void ManualFixedUpdate()
    {
        // attack state doesn't move by steering
    }

    private void StartSkill(Transform player)
    {
        _isExecuting = true;

        Vector2 dir = (player.position - _c.transform.position).normalized;
        _c.Data.SetLookDirection(dir);

        // actual skill execution
        _c.Combat.UseSkill(_currentSkill, dir);

        // after skill executed, we return to chase automatically
        _c.StartCoroutine(GoBackToChaseAfter(_currentSkill.Cooldown * 0.75f));
    }

    private System.Collections.IEnumerator GoBackToChaseAfter(float t)
    {
        yield return new WaitForSeconds(t);
        _isExecuting = false;
    }
}