using UnityEngine;

[CreateAssetMenu(menuName = "Game/Animation/Tag")]
public class AnimationTag : GameTagAsset
{
  public string Id => name;
}