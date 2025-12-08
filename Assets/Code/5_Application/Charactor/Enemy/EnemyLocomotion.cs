using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyLocomotion : MonoBehaviour
{
    public float baseSpeed = 3f;
    public float accel = 12f;
    public float turnSharpness = 10f;

    private Rigidbody2D rb;
    private Vector2 smoothedDir = Vector2.zero;
    private Vector2 currentVelocity = Vector2.zero;
    private bool isDash = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Apply steering result computed by Steering module. This method is the ONLY writer of rb.velocity.
    /// Call from FixedUpdate (EnemyController).
    /// </summary>
    public void ApplySteering(SteeringResult s)
    {
        if (isDash) return;

        Vector2 desired = s.desiredDir;
        if (desired.sqrMagnitude < 0.001f)
        {
            // no direction -> gently slow down
            currentVelocity = Vector2.MoveTowards(currentVelocity, Vector2.zero, accel * Time.fixedDeltaTime);
            rb.velocity = currentVelocity;
            return;
        }

        // smoothing direction
        smoothedDir = Vector2.Lerp(smoothedDir, desired, Time.deltaTime * turnSharpness);

        Vector2 targetVel = smoothedDir * baseSpeed * s.speedMul;
        currentVelocity = Vector2.MoveTowards(currentVelocity, targetVel, accel * Time.fixedDeltaTime);

        rb.velocity = currentVelocity;
    }

    public void ApplyImpulse(Vector2 impulse, float duration)
    {
        if (isDash) return;
        isDash = true;
        rb.velocity = impulse;
        StartCoroutine(EndDash(duration));
    }

    IEnumerator EndDash(float dur)
    {
        yield return new WaitForSeconds(dur);
        isDash = false;
        currentVelocity = Vector2.zero;
        rb.velocity = Vector2.zero;
    }

    public void Stop()
    {
        currentVelocity = Vector2.zero;
        rb.velocity = Vector2.zero;
    }

    public Vector2 GetVelocity() => rb.velocity;
}
