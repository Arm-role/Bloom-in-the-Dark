using TMPro;
using UnityEngine;

public class UpgradeModifierUI : MonoBehaviour
{
  [SerializeField] private TMP_Text beforeMod;
  [SerializeField] private TMP_Text afterMod;
  [SerializeField] private TMP_Text nameMod;

  public void Setup(IStatPreviewContext context, StatModifier statModifier)
  {
    float beforeValue = context.GetBefore(statModifier.StatKey);
    float afterValue = context.GetAfter(statModifier);

    beforeMod.text = beforeValue.ToString();
    afterMod.text = afterValue.ToString();
    nameMod.text = statModifier.StatKey.Id;
  }
}