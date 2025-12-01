using UnityEngine;

public class KnockbackSimulator : MonoBehaviour
{
    private Vector2 velocity;
    private float endTime;
    private float friction = 18f;

    private Rigidbody2D rb;
    public bool IsKnockbacking => Time.time < endTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void ApplyKnockback(Vector2 dir, float force, float duration)
    {
        dir.Normalize();
        velocity = dir * force;
        endTime = Time.time + duration;
    }

    private void Update()
    {
        if (!IsKnockbacking) return;

        float dt = Time.deltaTime;

        rb.velocity = velocity;

        velocity = Vector2.Lerp(velocity, Vector2.zero, friction * dt);

        if (Time.time >= endTime)
        {
            rb.velocity = Vector2.zero;
            velocity = Vector2.zero;
        }
    }
}
