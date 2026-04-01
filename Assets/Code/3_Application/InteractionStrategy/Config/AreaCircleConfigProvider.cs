using UnityEngine;

public sealed class AreaCircleConfigProvider : ITargetingConfigProvider
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

    if (rawPayload is not AreaCirclePayload payload)
      return null;

    // 4️⃣ Payload validation
    if (!payload.IsValid)
    {
      Debug.LogWarning("Invalid AreaCirclePayload detected.");
      return null;
    }

    return new AreaCircleConfig(
        payload.Range,
        payload.Radius,
        payload.XAngle
    );
  }
}