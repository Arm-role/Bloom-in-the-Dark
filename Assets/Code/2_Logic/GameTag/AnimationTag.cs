using UnityEngine;

[CreateAssetMenu(menuName = "Game/Animation/Tag")]
public class AnimationTag : ScriptableObject
{
  public string Id => name;

  private int _hash;
  public int Hash => _hash != 0 ? _hash : (_hash = Animator.StringToHash(name));
}