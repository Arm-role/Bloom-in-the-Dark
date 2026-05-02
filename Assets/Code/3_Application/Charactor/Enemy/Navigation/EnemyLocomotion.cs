using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyLocomotion : MonoBehaviour
{
  public float BaseSpeed { get; set; }
  public float Accel { get; set; }
  public float TurnSharpness { get; set; }
  public float KnockbackFriction { get; set; }

  public enum MovementMode
  {
    Normal,
    Dash,
    Slam,
    Knockback,
    Disabled
  }

  public event Action<Vector2> OnVelocityChanged;
  public MovementMode Mode { get; private set; } = MovementMode.Normal;

  private Rigidbody2D _rb;

  private Vector2 _smoothedDir = Vector2.zero;
  private Vector2 _currentVelocity = Vector2.zero;

  private Coroutine dashRoutine;

  // --- Knockback ---
  private Vector2 _knockbackVelocity;
  private float knockbackEndTime;

  void Awake()
  {
    _rb = GetComponent<Rigidbody2D>();
    _rb.interpolation = RigidbodyInterpolation2D.Interpolate;
  }

  private void FixedUpdate()
  {
    switch (Mode)
    {
      case MovementMode.Knockback:
        TickKnockback();
        return;

      case MovementMode.Dash:
      case MovementMode.Slam:
        return;
    }

    NotifyMovement();
  }

  // ======================================================
  // NORMAL AI MOVEMENT
  // ======================================================

  public void ApplySteering(SteeringResult s)
  {
    if (Mode != MovementMode.Normal) return;

    Vector2 desired = s.desiredDir;

    if (desired.sqrMagnitude < 0.001f)
    {
      _currentVelocity = Vector2.MoveTowards(
          _currentVelocity,
          Vector2.zero,
          Accel * Time.fixedDeltaTime);
    }
    else
    {
      _smoothedDir = Vector2.Lerp(
          _smoothedDir,
          desired,
          Time.fixedDeltaTime * TurnSharpness
      );

      Vector2 targetVel = _smoothedDir * BaseSpeed * s.speedMul;

      _currentVelocity = Vector2.MoveTowards(
          _currentVelocity,
          targetVel,
          Accel * Time.fixedDeltaTime
      );
    }

    _rb.velocity = _currentVelocity;
  }

  private void NotifyMovement()
  {
    Vector2 vel = _rb.velocity;
    OnVelocityChanged?.Invoke(vel);
  }


  // ======================================================
  // DASH
  // ======================================================

  public void ApplyDash(Vector2 impulse, float duration)
  {
    if (Mode != MovementMode.Normal)
      return;

    if (dashRoutine != null)
      StopCoroutine(dashRoutine);

    Mode = MovementMode.Dash;
    _rb.velocity = impulse;

    dashRoutine = StartCoroutine(EndDash(duration));
  }

  IEnumerator EndDash(float dur)
  {
    yield return new WaitForSeconds(dur);
    EndDashImmediate();
  }

  private void EndDashImmediate()
  {
    dashRoutine = null;
    _currentVelocity = Vector2.zero;
    _rb.velocity = Vector2.zero;
    Mode = MovementMode.Normal;
  }

  // ======================================================
  // Slam
  // ======================================================

  public void ApplySlam(Vector2 impulse, float duration)
  {
    if (Mode != MovementMode.Normal) return;

    if (dashRoutine != null) StopCoroutine(dashRoutine);

    Mode = MovementMode.Slam;
    _rb.velocity = impulse;

    dashRoutine = StartCoroutine(EndSlam(duration));
  }

  private IEnumerator EndSlam(float dur)
  {
    yield return new WaitForSeconds(dur);
    _currentVelocity = Vector2.zero;
    _rb.velocity = Vector2.zero;
    Mode = MovementMode.Normal;
    dashRoutine = null;
  }

  // ======================================================
  // KNOCKBACK
  // ======================================================

  public void ApplyKnockback(Vector2 dir, float force, float duration)
  {
    if (Mode == MovementMode.Dash || Mode == MovementMode.Disabled)
      return;

    dir.Normalize();

    _knockbackVelocity = dir * force;
    knockbackEndTime = Time.time + duration;

    Mode = MovementMode.Knockback;
  }

  private void TickKnockback()
  {
    _rb.velocity = _knockbackVelocity;

    _knockbackVelocity = Vector2.MoveTowards(
        _knockbackVelocity,
        Vector2.zero,
        KnockbackFriction * Time.fixedDeltaTime
    );

    if (Time.time >= knockbackEndTime || _knockbackVelocity.sqrMagnitude < 0.01f)
    {
      _knockbackVelocity = Vector2.zero;
      _rb.velocity = Vector2.zero;
      Mode = MovementMode.Normal;
    }
  }

  // ======================================================
  // CONTROL
  // ======================================================

  public void StopMovement()
  {
    _currentVelocity = Vector2.zero;

    if (Mode == MovementMode.Normal)
      _rb.velocity = Vector2.zero;
  }

  public void StopKnockback()
  {
    if (Mode != MovementMode.Knockback)
      return;

    _knockbackVelocity = Vector2.zero;
    _rb.velocity = Vector2.zero;
    Mode = MovementMode.Normal;
  }

  public void HardStop()
  {
    _currentVelocity = Vector2.zero;
    _knockbackVelocity = Vector2.zero;
    _rb.velocity = Vector2.zero;
    Mode = MovementMode.Disabled;
  }

  public void DisableMovement()
  {
    StopMovement();
    Mode = MovementMode.Disabled;
  }

  public void EnableMovement()
  {
    Mode = MovementMode.Normal;
  }

  // ======================================================
  // DEBUG / QUERY
  // ======================================================

  public Vector2 Velocity => _rb.velocity;
  public bool IsMoving => _rb.velocity.sqrMagnitude > 0.01f;
  public bool IsKnockbacking => Mode == MovementMode.Knockback;
}
