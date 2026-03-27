public readonly struct ItemCooldownFeedback
{
  public string Key { get; }
  public float Duration { get; }

  public bool HasCost =>
      Key != string.Empty || Duration > 0f;

  public ItemCooldownFeedback(
    string key,
    float cooldown = 0f)
  {
    Key = key;
    Duration = cooldown;
  }

  public static ItemCooldownFeedback None
      => new ItemCooldownFeedback(string.Empty);

  public static ItemCooldownFeedback Consumed(
    string key,
    float cooldown = 0f)
      => new ItemCooldownFeedback(key, cooldown);
}