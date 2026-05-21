using UnityEngine;

public sealed class AreaLineConfigProvider : ITargetingConfigProvider
{
  public ITargetingConfig Create(InteractionHandleContext ctx)
  {
    var itemInstance = ctx.ItemInstance;

    var data = itemInstance.Data;
    if (data == null)
      return null;

    if (data.Skill == null)
      return null;

    // 3️⃣ Execute safety wrapper
    if (!data.Skill.Execute(itemInstance, out ISkillDataPayload rawPayload))
      return null;

    // Strong type check
    if (rawPayload is not IAreaLinePayload payload)
    {
      Debug.LogWarning("AreaLine skill returned wrong payload type.");
      return null;
    }

    // Validate struct
    if (!payload.IsValid)
    {
      Debug.LogWarning("Invalid LineAttackPayload detected.");
      return null;
    }

    return new AreaLineConfig(
      xAngle: ctx.PlayerDirection?.x ?? 0f, // หรือ derive จาก direction logic
      length: payload.Range,
      width: payload.Width
    );
  }
}