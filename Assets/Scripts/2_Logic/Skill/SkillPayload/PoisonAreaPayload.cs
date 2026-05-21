public struct PoisonAreaPayload : IAreaCirclePayload
{
  public float DamagePerTick { get; set; }
  public float Radius { get; set; }
  public float Duration { get; set; }
  public float TickInterval { get; set; }
  public float Range { get; set; }
  public float Cooldown { get; set; }
  public float XAngle { get; set; }

  public bool IsValid =>
    DamagePerTick > 0f &&
    Radius > 0f &&
    Duration > 0f &&
    TickInterval > 0f &&
    Cooldown > 0f &&
    XAngle > 0f &&
    !float.IsNaN(DamagePerTick) &&
    !float.IsNaN(Radius) &&
    !float.IsNaN(Duration) &&
    !float.IsNaN(TickInterval) &&
    !float.IsNaN(Cooldown) &&
    !float.IsNaN(XAngle);
}
