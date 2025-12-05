using UnityEngine;

[DefaultExecutionOrder(-50)]
public class EnemyCrowdAgent : MonoBehaviour
{
    [Header("Runtime")]
    public Vector2 DesiredVelocity;   // velocity จาก AI (pathfinder)
    public Vector2 FinalVelocity;     // velocity หลังเลี่ยงเพื่อนบ้าน
    public bool EnableAvoidance = true;

    [Header("Agent Data")]
    public float Radius = 0.5f;
    public float MaxSpeed = 3f;

    private Rigidbody2D _rb;
    private BoxCollider2D _box;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _box = GetComponent<BoxCollider2D>();

        if (_box != null)
            Radius = Mathf.Max(_box.size.x, _box.size.y) * 0.5f;

        CrowdManager.Register(this);
    }

    void OnDestroy()
    {
        CrowdManager.Unregister(this);
    }

    void FixedUpdate()
    {
        Vector2 vel = FinalVelocity;

        if (EnableAvoidance == false)
            vel = DesiredVelocity;

        vel = Vector2.ClampMagnitude(vel, MaxSpeed);

        _rb.velocity = vel;
    }
}