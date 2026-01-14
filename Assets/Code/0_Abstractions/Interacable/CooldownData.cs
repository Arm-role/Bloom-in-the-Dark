using UnityEngine;

public readonly struct CooldownData
{
    public readonly float StartTime;
    public readonly float Duration;

    public CooldownData(float startTime, float duration)
    {
        StartTime = startTime;
        Duration = duration;
    }

    public float EndTime => StartTime + Duration;

    public float RemainingTime =>
        Mathf.Max(0f, EndTime - Time.time);

    public float Normalized =>
        Duration <= 0f ? 0f : RemainingTime / Duration;

    public bool IsActive =>
        Time.time < EndTime;
}