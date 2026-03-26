using UnityEngine;

public interface ISkillExecutor
{
  bool Initialize(
    Vector2 origin,
    Vector2 direction,
    ISkillDataPayload payload,
    GameObject owner,
    InteractionIntent intent);
}