
using UnityEngine;

[CreateAssetMenu(menuName = "Interaction/new IntentMatchRule")]
public class InteractionIntentMatchRule : ScriptableObject
{
  [SerializeField] private EInteractionIntentType intent;
  [SerializeField] private EItemCategory category;
  [SerializeField] private EItemRole itemRole;
  [SerializeField] private ETargetType targetMask;

  public EInteractionIntentType Intent => intent;
  public EItemCategory Category => category;
  public EItemRole ItemRole => itemRole;
  public ETargetType TargetMask => targetMask;
}