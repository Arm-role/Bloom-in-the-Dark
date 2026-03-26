using UnityEngine;

public interface ISkill
{
  void Cast(GameObject owner, InteractionIntent intent, Vector2 pos);
}