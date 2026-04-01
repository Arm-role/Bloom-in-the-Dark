using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FloatingTextService
{
  private readonly IAdressablePoolService<GameObject> _pool;
  private readonly GameObject _prefab;
  private readonly Dictionary<FloatingTextType, IFloatingTextStyleConfig> _styles;

  public FloatingTextService(
      IAdressablePoolService<GameObject> pool,
      GameObject prefab,
      List<IFloatingTextStyleConfig> styles)
  {
    _pool = pool;

    _prefab = prefab;

    _styles = new();

    foreach (var s in styles)
      _styles[s.Tpye] = s;
  }

  public async Task Spawn(
      FloatingTextType type,
      Vector3 worldPos,
      int value)
  {
    if (!_styles.TryGetValue(type, out var style))
    {
      Debug.LogWarning($"Missing style config for {type}");
      return;
    }

    var instance = await _pool.AsyncGet(_prefab);

    instance.transform.position =
        worldPos + GetRandomOffset();

    var popup = instance.GetComponent<IFloatingTextView>();

    popup.Setup(
        Format(type, value),
        style.Color,
        style.Lifetime,
        style.OpacityCurve,
        style.ScaleCurve,
        style.HightCurve,
        Release
    );
  }

  private string Format(
      FloatingTextType type,
      int value)
  {
    return type switch
    {
      FloatingTextType.Heal => "+" + value,
      _ => value.ToString()
    };
  }

  private Vector3 GetRandomOffset()
  {
    return new Vector3(
        Random.Range(-0.25f, 0.25f),
        Random.Range(0.15f, 0.45f),
        0
    );
  }

  private void Release(GameObject popup)
  {
    _pool.Return(
        _prefab,
        popup
    );
  }
}