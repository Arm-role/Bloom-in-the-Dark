using UnityEngine;

public sealed class ConeConfigProvider : ITargetingConfigProvider
{
  public ITargetingConfig Create(InteractionHandleContext ctx)
  {
    var itemInstance = ctx.ItemInstance;

    var data = itemInstance.Data;
    if (data == null)
      return null;

    if (data.Skill == null)
      return null;

    if (!data.Skill.Execute(itemInstance, out ISkillDataPayload rawPayload))
      return null;

    if (rawPayload is not ConeAttackPayload payload)
      return null;

    if (!payload.IsValid)
    {
      Debug.LogWarning("Invalid ConeAttackPayload detected.");
      return null;
    }

    return new ConeConfig(
        payload.XAngle,
        payload.Range,
        payload.AngleDeg
    );
  }
}