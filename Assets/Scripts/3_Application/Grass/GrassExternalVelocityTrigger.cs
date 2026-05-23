#nullable enable
using UnityEngine;

public sealed class GrassExternalVelocityTrigger : MonoBehaviour
{
  // ถือว่า "settled" เมื่อ _currentInfluence ห่างจาก _baseInfluence น้อยกว่านี้ —
  // หลังจากนั้น ถ้ายังอยู่นอกรัศมีก็ข้าม shader push ได้ทั้งเฟรม.
  private const float SettledEpsilon = 0.001f;

  [SerializeField] private SpriteRenderer[] renderers = null!;

  private GrassVelocityController _controller = null!;
  private Transform? _player;
  private Rigidbody2D? _playerRb;

  private MaterialPropertyBlock _mpb = null!;

  private float _currentInfluence;
  private float _baseInfluence;

  private bool _wasInside;
  private int _externInfluence;

  // --------------------------------------------------
  // Setup
  // --------------------------------------------------

  private void Awake()
  {
    _controller = GetComponent<GrassVelocityController>();
    _mpb = new MaterialPropertyBlock();

    _externInfluence = Shader.PropertyToID("_ExternInfluence");

    if (renderers.Length > 0 && renderers[0] != null)
    {
      _baseInfluence =
          renderers[0].sharedMaterial.GetFloat(_externInfluence);
    }
    else
    {
      _baseInfluence = 1f; // fallback
    }

    _currentInfluence = _baseInfluence;
  }

  private void Start()
  {
    GameObject player = GameObject.FindGameObjectWithTag("Player");
    if (player == null) return;

    _player = player.transform;
    _playerRb = player.GetComponent<Rigidbody2D>();
  }

  // --------------------------------------------------
  // Update
  // --------------------------------------------------

  private void Update()
  {
    if (_player == null || _playerRb == null) return;

    Vector2 grassPos = transform.position;
    Vector2 playerPos = _player.position;
    Vector2 offset = grassPos - playerPos;        // grass relative to player
    float distSqr = offset.sqrMagnitude;
    float radius = _controller.InfluenceRadius;
    bool inside = distSqr <= radius * radius;

    // Settled at base + ยังอยู่นอกรัศมีตั้งแต่เฟรมก่อน → ไม่มีอะไรเปลี่ยน, ข้าม shader push.
    if (!inside && !_wasInside &&
        Mathf.Abs(_currentInfluence - _baseInfluence) < SettledEpsilon)
      return;

    float target = _baseInfluence;

    if (inside)
    {
      Vector2 velocity = _playerRb.velocity;
      float speed = velocity.magnitude;

      if (speed > _controller.VelocityThreshold)
      {
        // sqrt เฉพาะตอนที่ใช้จริง (inside + เร็วพอ).
        float distance = Mathf.Sqrt(distSqr);
        float normalizedDist = 1f - (distance / radius);

        // quadratic falloff
        float falloff = normalizedDist * normalizedDist;

        Vector2 dir = velocity.normalized;
        Vector2 toGrass = offset.normalized;     // direction from player toward grass
        float directional = Vector2.Dot(dir, toGrass);

        float influence =
            speed *
            directional *
            falloff *
            _controller.ExternInfluenceStrength;

        if (speed > _controller.ImpactVelocityThreshold)
          influence *= _controller.ImpactMultiplier;

        target = _baseInfluence + influence;
      }

      if (!_wasInside)
        _currentInfluence += _playerRb.velocity.magnitude * 0.3f;
    }

    _wasInside = inside;

    ApplyInfluence(target);
    ApplyToRenderers(_currentInfluence);
  }

  // --------------------------------------------------
  // Stylized Smooth (Not Physics)
  // --------------------------------------------------

  private void ApplyInfluence(float target)
  {
    float speed = _controller.EaseSpeed;

    _currentInfluence = Mathf.Lerp(
        _currentInfluence,
        target,
        speed * Time.deltaTime
    );
  }

  // --------------------------------------------------
  // Apply To All Renderers
  // --------------------------------------------------

  private void ApplyToRenderers(float value)
  {
    for (int i = 0; i < renderers.Length; i++)
    {
      var r = renderers[i];
      if (r == null) continue;

      r.GetPropertyBlock(_mpb);
      _mpb.SetFloat(_controller.ExternInfluenceID, value);
      r.SetPropertyBlock(_mpb);
    }
  }

#if UNITY_EDITOR
  // --------------------------------------------------
  // Gizmos
  // --------------------------------------------------

  private void OnDrawGizmosSelected()
  {
    if (_controller == null)
      _controller = GetComponent<GrassVelocityController>();

    if (_controller == null)
      return;

    // Influence Radius
    Gizmos.color = new Color(0f, 0.6f, 1f, 0.3f);
    Gizmos.DrawWireSphere(transform.position, _controller.InfluenceRadius);

    // Current influence direction
    Gizmos.color = Color.yellow;
    Vector3 dir = Vector3.right * (_currentInfluence - _baseInfluence);
    Gizmos.DrawLine(transform.position, transform.position + dir);

    // Player relation
    if (_player != null)
    {
      float distance = Vector2.Distance(_player.position, transform.position);
      bool inside = distance <= _controller.InfluenceRadius;

      Gizmos.color = inside ? Color.green : Color.red;
      Gizmos.DrawLine(transform.position, _player.position);
      Gizmos.DrawSphere(_player.position, 0.08f);
    }
  }
#endif
}
