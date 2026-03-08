using UnityEngine;

public class GrassExternalVelocityTrigger : MonoBehaviour
{
   [SerializeField] private SpriteRenderer[] renderers;

    private GrassVelocityController _controller;
    private Transform _player;
    private Rigidbody2D _playerRb;

    private MaterialPropertyBlock _mpb;

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
        if (_player == null) return;

        float distance = Vector2.Distance(_player.position, transform.position);
        bool inside = distance <= _controller.InfluenceRadius;

        float target = _baseInfluence;

        if (inside)
        {
            float velocity = _playerRb.velocity.x;

            if (Mathf.Abs(velocity) > _controller.VelocityThreshold)
            {
                float normalizedDist = 1f - (distance / _controller.InfluenceRadius);

                // quadratic falloff (stronger near center)
                float falloff = normalizedDist * normalizedDist;

                float influence = velocity
                                  * falloff
                                  * _controller.ExternInfluenceStrength;

                // optional boost for high speed
                if (Mathf.Abs(velocity) > _controller.ImpactVelocityThreshold)
                {
                    influence *= _controller.ImpactMultiplier;
                }

                target = _baseInfluence + influence;
            }

            // small snap when entering
            if (!_wasInside)
            {
                _currentInfluence += _playerRb.velocity.x * 0.3f;
            }
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
