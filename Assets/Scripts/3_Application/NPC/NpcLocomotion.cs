using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class NpcLocomotion : MonoBehaviour
{
    public float MoveSpeed = 3f;
    public float Accel = 15f;

    private Rigidbody2D _rb;
    private Vector2 _currentVelocity;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void ApplySteering(SteeringResult s)
    {
        Vector2 target = s.desiredDir.sqrMagnitude > 0.001f
            ? s.desiredDir * MoveSpeed * s.speedMul
            : Vector2.zero;

        _currentVelocity = Vector2.MoveTowards(_currentVelocity, target, Accel * Time.fixedDeltaTime);
        _rb.velocity = _currentVelocity;
    }

    public void Stop()
    {
        _currentVelocity = Vector2.MoveTowards(_currentVelocity, Vector2.zero, Accel * Time.fixedDeltaTime);
        _rb.velocity = _currentVelocity;
    }
}
