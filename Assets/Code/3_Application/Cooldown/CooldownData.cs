using UnityEngine;

public readonly struct CooldownData
{
  private readonly float _startTime;
  private readonly float _duration;
  private readonly ITimeSource _time;

  public CooldownData(
      float startTime,
      float duration,
      ITimeSource time)
  {
    _startTime = startTime;
    _duration = duration;
    _time = time;
  }

  public float Remaining
      => Mathf.Max(0f, (_startTime + _duration) - _time.Now);

  public float Normalized
      => _duration <= 0f ? 0f : Remaining / _duration;

  public bool IsActive => Remaining > 0f;
}