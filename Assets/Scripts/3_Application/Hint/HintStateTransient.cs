#nullable enable

using System.Collections.Generic;
#nullable enable
public sealed class HintStateTransient : IHintState
{
  private readonly HashSet<string> _viewed = new();
  private readonly HashSet<string> _unlocked = new();

  public bool IsViewed(string id) => _viewed.Contains(id);
  public void MarkViewed(string id) => _viewed.Add(id);

  public bool IsUnlocked(string id) => _unlocked.Contains(id);
  public void Unlock(string id) => _unlocked.Add(id);

  public bool IsWelcomeShown => false;  // always false
  public void MarkWelcomeShown() { }  // no-op
}