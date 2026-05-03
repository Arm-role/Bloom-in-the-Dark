using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FloatingText/Config")]
public class FloatingTextConfig : ScriptableObject
{
  [SerializeField] private GameObject prefab;
  [SerializeField] private FloatingTextStyleConfig[] styles;

  public GameObject Prefab => prefab;

  public List<IFloatingTextStyleConfig> Styles
  {
    get
    {
      return new List<IFloatingTextStyleConfig>(styles);
    }
  }
}