using UnityEngine;

[CreateAssetMenu(menuName = "FloatingText/Style Config")]
public class FloatingTextStyleConfig : ScriptableObject, IFloatingTextStyleConfig
{
  [SerializeField] private FloatingTextType type;
  [SerializeField] private Color color = Color.white;
  [SerializeField] private float lifetime = 1f;

  [Header("Animation Curve")]
  [SerializeField] private AnimationCurve _opacityCurve;
  [SerializeField] private AnimationCurve _scaleCurve;
  [SerializeField] private AnimationCurve _hightCurve;

  public FloatingTextType Tpye => type;
  public Color Color => color;
  public float Lifetime => lifetime;
  public AnimationCurve OpacityCurve => _opacityCurve;
  public AnimationCurve ScaleCurve => _scaleCurve;
  public AnimationCurve HightCurve => _hightCurve;
}