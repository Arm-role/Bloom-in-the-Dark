using System.Collections;
using UnityEngine;

public class GrassExternalVelocityTrigger : MonoBehaviour
{
    private GrassVelocityController _grassVelocityController;
    private GameObject _player;
    private Material _material;
    private Rigidbody2D _playerRb;

    private Coroutine _currentCoroutine;
    private int _externInfluence = Shader.PropertyToID("_ExternInfluence");

    private float _baseInfluence;
    private float _velocityLastFrame;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerRb = _player.GetComponent<Rigidbody2D>();
        _grassVelocityController = GetComponentInParent<GrassVelocityController>();
        _material = GetComponent<SpriteRenderer>().material;

        _baseInfluence = _material.GetFloat(_externInfluence);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == _player)
        {
            float vel = _playerRb.velocity.x;
            if (Mathf.Abs(vel) > _grassVelocityController.VelocityThreshold)
                StartNewCoroutine(EaseIn(vel * _grassVelocityController.ExternInfluenceStrength));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == _player)
            StartNewCoroutine(SpringReturn());
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject != _player) return;

        float currentVel = _playerRb.velocity.x;

        if (Mathf.Abs(_velocityLastFrame) > _grassVelocityController.VelocityThreshold &&
            Mathf.Abs(currentVel) < _grassVelocityController.VelocityThreshold)
        {
            StartNewCoroutine(SpringReturn());
        }
        else if (Mathf.Abs(_velocityLastFrame) < _grassVelocityController.VelocityThreshold &&
                 Mathf.Abs(currentVel) > _grassVelocityController.VelocityThreshold)
        {
            StartNewCoroutine(EaseIn(currentVel * _grassVelocityController.ExternInfluenceStrength));
        }

        _velocityLastFrame = currentVel;
    }

    private void StartNewCoroutine(IEnumerator coroutine)
    {
        if (_currentCoroutine != null)
            StopCoroutine(_currentCoroutine);

        _currentCoroutine = StartCoroutine(coroutine);
    }

    private IEnumerator EaseIn(float target)
    {
        if (target > 0)
            target += 1f;

        float start = _material.GetFloat(_externInfluence);
        float elapsed = 0f;

        while (elapsed < _grassVelocityController.EaseInTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _grassVelocityController.EaseInTime;
            float curvedT = _grassVelocityController.EaseCurve.Evaluate(t);
            float newVal = Mathf.Lerp(start, target, curvedT);  
            _grassVelocityController.InfluenceGrass(_material, newVal);
            yield return null;
        }
    }

    private IEnumerator SpringReturn()
    {
        float current = _material.GetFloat(_externInfluence);
        float velocity = 0f;

        float damping = _grassVelocityController.SpringDamping;  // แรงหน่วง
        float stiffness = _grassVelocityController.SpringStiffness;  // ความแข็งของสปริง

        while (Mathf.Abs(current - _baseInfluence) > 0.001f || Mathf.Abs(velocity) > 0.001f)
        {
            float force = -stiffness * (current - _baseInfluence);
            velocity += force * Time.deltaTime;
            velocity *= (1f - damping * Time.deltaTime);

            current += velocity * Time.deltaTime;
            _grassVelocityController.InfluenceGrass(_material, current);
            yield return null;
        }

        _grassVelocityController.InfluenceGrass(_material, _baseInfluence);
    }
}