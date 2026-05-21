public struct BeamPayload : IAreaLinePayload
{
  public float DamagePerTick { get; set; }
  public float Range { get; set; }
  public float Width { get; set; }
  public float Duration { get; set; }
  public float TickInterval { get; set; }
  public float KnockForce { get; set; }
  public float KnockDuration { get; set; }
  public float Cooldown { get; set; }

  public bool IsValid =>
    DamagePerTick > 0f &&
    Range > 0f &&
    Width > 0f &&
    Duration > 0f &&
    TickInterval > 0f &&
    Cooldown > 0f &&
    !float.IsNaN(DamagePerTick) &&
    !float.IsNaN(Range) &&
    !float.IsNaN(Width) &&
    !float.IsNaN(Duration) &&
    !float.IsNaN(TickInterval) &&
    !float.IsNaN(Cooldown);
}
