using System;
using TMPro;
using UnityEngine;

public class FloatingTextView : MonoBehaviour, IFloatingTextView
{
  [SerializeField] private TMP_Text _text;

  private Color _color;
  private float _lifetime;
  private float _time;

  private AnimationCurve _opacityCurve;
  private AnimationCurve _scaleCurve;
  private AnimationCurve _hightCurve;

  private Vector3 _origin;

  private Camera _camera;

  private Action<GameObject> _onReturn;
  public void Setup(
         string message,
         Color color,
         float lifetime,
         AnimationCurve opacity,
         AnimationCurve scale,
         AnimationCurve height,
         Action<GameObject> onReturn)
  {
    _text.text = message;

    _color = color;

    _lifetime = lifetime;

    _opacityCurve = opacity;
    _scaleCurve = scale;
    _hightCurve = height;

    _time = 0f;

    _origin = transform.position; 

    _camera = Camera.main;

    _onReturn = onReturn;

    gameObject.SetActive(true);
  }

  private void Update()
  {
    float normalizedTime = _time / _lifetime;

    if (normalizedTime >= 1f)
    {
      _onReturn?.Invoke(gameObject);
      return;
    }

    float alpha = _opacityCurve.Evaluate(normalizedTime);

    _text.color = new Color(
        _color.r,
        _color.g,
        _color.b,
        alpha
    );

    float scale = _scaleCurve.Evaluate(normalizedTime);

    transform.localScale = Vector3.one * scale;

    float height = _hightCurve.Evaluate(normalizedTime);

    transform.position = _origin + Vector3.up * height;

    _time += Time.deltaTime;
  }

  private void LateUpdate()
  {
    if (_camera == null)
      return;

    transform.forward = _camera.transform.forward;
  }

  private void OnDisable()
  {
    _time = 0f;
  }
}
