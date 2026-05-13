using UnityEngine;

public abstract class GameTagAsset : ScriptableObject
{
  [SerializeField] private GameTagAsset parent;

  private GameTag _runtimeTag;

  public GameTag RuntimeTag
  {
    get
    {
      if (_runtimeTag.Equals(default))
        _runtimeTag = new GameTag(name);
      return _runtimeTag;
    }
  }

  public GameTagAsset Parent => parent;
}
