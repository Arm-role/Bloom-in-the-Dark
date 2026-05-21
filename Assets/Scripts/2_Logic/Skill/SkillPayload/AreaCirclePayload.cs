public struct AreaCirclePayload : IAreaCirclePayload
{
  public float Damage { get; set; }
  public float Range { get; set; }
  public float Radius { get; set; }
  public float KnockForce { get; set; }
  public float KnockDuration { get; set; }
  public float Duration { get; set; }
  public float Cooldown { get; set; }
  public float XAngle { get; set; }

  public bool IsValid =>
    Range > 0f &&
    Radius > 0f &&
    XAngle > 0f &&
    Duration > 0f &&
    Cooldown > 0f &&
    !float.IsNaN(Range) &&
    !float.IsNaN(Radius) &&
    !float.IsNaN(Duration) &&
    !float.IsNaN(Cooldown) &&
    !float.IsNaN(XAngle);
}
