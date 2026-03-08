using UnityEngine;

public abstract class GameTagAsset : ScriptableObject
{
  [SerializeField] private string id;

  public string Id => id;

  private GameTag _runtimeTag;

  public GameTag RuntimeTag
  {
    get
    {
      if (_runtimeTag.Equals(default))
        _runtimeTag = new GameTag(id);
      return _runtimeTag;
    }
  }

#if UNITY_EDITOR
  private void OnValidate()
  {
    id = name; // sync asset name
  }
#endif
}