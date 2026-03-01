using UnityEngine;

public class SkillSelfController
{
  private readonly PlayerInteractor _playerInteractor;

  public SkillSelfController(PlayerInteractor playerInteractor)
  {
    _playerInteractor = playerInteractor;
  }

  public void Use(IItemInstance item, InteractionIntent _)
  {
    if (item.Data.Skill.Execute(item, out var p))
    {
      if (p is IncreaseMaxEnergyPayload payload)
      {
        _playerInteractor.TryExecute(
          new IncreaseMaxEnergyCommand(payload.Increase));
      }
    }
  }
}