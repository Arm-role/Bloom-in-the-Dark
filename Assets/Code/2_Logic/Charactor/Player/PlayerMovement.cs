using UnityEngine;

public class PlayerMovement : IMovement
{
  private readonly StatKey _speedKey;
  private readonly IStatService _statService;
  public PlayerMovement(StatKey speedKey, IStatService statService)
  {
    _speedKey = speedKey;
    _statService = statService;
  }
  public Vector2 CalculateVelocity(Vector2 direction)
  {
    return direction * _statService.GetStat(_speedKey);
  }
}
