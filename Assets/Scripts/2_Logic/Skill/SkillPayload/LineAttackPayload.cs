public struct LineAttackPayload : IAreaLinePayload
{
  public float Damage { get; set; }
  public float Range { get; set; }
  public float Width { get; set; }
  public float KnockForce { get; set; }
  public float KnockDuration { get; set; }
  public float Duration { get; set; }
  public float Cooldown { get; set; }
  
  public bool IsValid =>
    Width > 0f &&
    Range > 0f &&
    Duration > 0f &&
    Cooldown > 0f &&
    !float.IsNaN(Range) &&
    !float.IsNaN(Width) &&
    !float.IsNaN(Duration) &&
    !float.IsNaN(Cooldown) &&
    !float.IsInfinity(Range) &&
    !float.IsInfinity(Width);

}