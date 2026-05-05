public struct ConeAttackPayload : ISkillDataPayload
{
  public float Damage { get; set; }
  public float Range { get; set; }
  public float AngleDeg { get; set; } // full cone angle e.g. 90°
  public float KnockForce { get; set; }
  public float KnockDuration { get; set; }
  public float Duration { get; set; }
  public float Cooldown { get; set; }
  public float XAngle { get; set; } // perspective tilt (ใช้ yScale)

  public bool IsValid =>
      Range > 0f &&
      AngleDeg > 0f &&
      Duration > 0f &&
      Cooldown > 0f &&
      XAngle > 0f &&
      !float.IsNaN(Range) &&
      !float.IsNaN(AngleDeg) &&
      !float.IsNaN(Duration) &&
      !float.IsNaN(Cooldown) &&
      !float.IsNaN(XAngle);
}