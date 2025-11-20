using UnityEngine;

public class KnockbackSimulator : MonoBehaviour
{
    private Vector2 velocity;
    private float endTime;
    private float friction = 18f;

    public void ApplyKnockback(Vector2 dir, float force, float duration)
    {
        dir.Normalize();
        velocity = dir * force;
        endTime = Time.time + duration;
    }

    private void Update()
    {
        if (velocity.sqrMagnitude < 0.0001f) return;

        float dt = Time.deltaTime;

        transform.position += (Vector3)(velocity * dt);

        velocity = Vector2.Lerp(velocity, Vector2.zero, friction * dt);

        if (Time.time >= endTime)
            velocity = Vector2.zero;
    }
}
