using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemModules/Interaction/Profile")]
public class ItemInteractionProfile : ScriptableObject
{
  [SerializeField] private float interactionRange;
  [SerializeField] private float interactionDamage;
  [SerializeField] private EInteractionIntentType[] supportedIntents;

  public float Range => interactionRange;
  public float Damage => interactionDamage;
  public IReadOnlyList<EInteractionIntentType> SupportedIntents => supportedIntents;
}