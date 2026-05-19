/// <summary>
/// Runtime flags for the current play session.
/// Resets on app restart (not persistent).
/// </summary>
public static class GameSession
{
  public static bool IsEndlessMode { get; set; } = false;
  public static int EndlessStartDay { get; set; } = 51;

  public static void Reset()
  {
    IsEndlessMode = false;
  }
}
