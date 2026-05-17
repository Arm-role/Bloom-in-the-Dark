
using UnityEngine;

[CreateAssetMenu(menuName = "Interaction/new IntentMatchRule")]
public class InteractionIntentMatchRule : ScriptableObject
{
  [SerializeField] private EInteractionIntentType intent;

  [Header("Item Requirements")]
  [SerializeField] private ItemTag[] requiredItemTags;
  [SerializeField] private ETargetType targetMask;

  public EInteractionIntentType Intent => intent;

  public bool Match(
        EInteractionIntentType intent,
        GameTagContainer itemTags,
        ETargetType targetMask)
  {
    if (!Intent.Equals(intent))
    {
      //Debug.Log($"{Intent} != {intent}");
      return false;
    }

    foreach (var tag in requiredItemTags)
    {
      if (!itemTags.Has(tag.RuntimeTag))
        return false;
    }

    //Debug.Log(targetMask);
    if ((this.targetMask & targetMask) == 0)
      return false;


    return true;
  }
}