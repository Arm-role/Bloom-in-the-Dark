#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

// Editor menu สำหรับ reset hint state — ใช้ทดสอบ welcome popup ซ้ำได้
// Menu path: Tools → Hint → ...
public static class HintDebugMenu
{
  [MenuItem("Tools/Hint/Reset All Hint PlayerPrefs")]
  public static void ResetAll()
  {
    PlayerPrefs.DeleteKey("bloom_hint_viewed");
    PlayerPrefs.DeleteKey("bloom_hint_unlocked");
    PlayerPrefs.DeleteKey("bloom_hint_welcome_shown");
    PlayerPrefs.Save();
    Debug.Log("[Hint] Reset complete: viewed + unlocked + welcome cleared");
  }

  [MenuItem("Tools/Hint/Reset Welcome Only")]
  public static void ResetWelcomeOnly()
  {
    PlayerPrefs.DeleteKey("bloom_hint_welcome_shown");
    PlayerPrefs.Save();
    Debug.Log("[Hint] Welcome flag cleared (viewed + unlocked preserved)");
  }

  [MenuItem("Tools/Hint/Print Current State")]
  public static void PrintState()
  {
    var viewed = PlayerPrefs.GetString("bloom_hint_viewed", "(empty)");
    var unlocked = PlayerPrefs.GetString("bloom_hint_unlocked", "(empty)");
    var welcome = PlayerPrefs.GetInt("bloom_hint_welcome_shown", 0);
    Debug.Log(
      $"[Hint] State\n" +
      $"  welcome_shown = {welcome}\n" +
      $"  viewed = {viewed}\n" +
      $"  unlocked = {unlocked}");
  }
}
#endif
