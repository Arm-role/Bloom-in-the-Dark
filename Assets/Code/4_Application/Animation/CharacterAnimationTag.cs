using UnityEngine;

[CreateAssetMenu(menuName = "Animation/Tag")]
public class CharacterAnimationTag : ScriptableObject
{
  [SerializeField] private string triggerName;

  private void OnValidate()
  {
    triggerName = name;
  }

  private int _hash;

  public int Hash
  {
    get
    {
      if (_hash == 0)
        _hash = Animator.StringToHash(triggerName);
      return _hash;
    }
  }
}