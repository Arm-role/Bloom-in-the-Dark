public class SkillSelfController
{
  private readonly PlayerInteractor _playerInteractor;

  public SkillSelfController(PlayerInteractor playerInteractor)
  {
    _playerInteractor = playerInteractor;
  }

  public void Use(ISkillDataPayload payload, InteractionIntent _)
  {
    if (payload is IncreaseMaxEnergyPayload increaseMaxEnergypayload)
    {
      _playerInteractor.TryExecute(
        new IncreaseMaxEnergyCommand(increaseMaxEnergypayload.Increase));
    }
  }
}