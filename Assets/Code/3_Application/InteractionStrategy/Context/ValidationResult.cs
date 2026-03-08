public struct ValidationResult
{
    public bool IsValid;
    public string Reason;   // optional: อธิบายสาเหตุ (ดีมากตอน debug หรือ UI)
    public ETileLayerType? TargetLayer; // optional: ระบุ layer ที่จะทำงาน

    public static ValidationResult Success()
        => new ValidationResult { IsValid = true };

    public static ValidationResult Fail(string reason)
        => new ValidationResult { IsValid = false, Reason = reason };
}