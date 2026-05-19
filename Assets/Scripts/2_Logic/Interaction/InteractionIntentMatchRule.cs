
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

    // null array = empty array = "no item requirement" (rule matches by intent + target)
    if (requiredItemTags != null && requiredItemTags.Length > 0)
    {
      if (itemTags == null) return false;

      for (int i = 0; i < requiredItemTags.Length; i++)
      {
        var tag = requiredItemTags[i];
        if (tag == null)
        {
          Debug.LogError($"[IntentMatchRule:{name}] requiredItemTags[{i}] is null — broken ItemTag asset reference");
          return false;
        }
        if (!itemTags.Has(tag.RuntimeTag))
          return false;
      }
    }

    //Debug.Log(targetMask);
    if ((this.targetMask & targetMask) == 0)
      return false;


    return true;
  }
}