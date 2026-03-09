using UnityEngine;

[CreateAssetMenu(menuName = "Item/Key")]
public class ItemKey : ScriptableObject
{
  public string Id => name;

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
}
