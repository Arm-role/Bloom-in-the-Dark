#nullable enable

using System.Collections.Generic;
using UnityEngine;

// PlayerPrefs impl ของ IHintState — เก็บ Set<string> สอง set: viewed + unlocked
// ใช้ pipe ('|') เป็น delimiter (id ไม่ควรมี pipe — enforce ด้วย convention)
// In-memory cache HashSet เพื่อ lookup O(1) — flush PlayerPrefs ตอน mutate เท่านั้น
public sealed class HintState : IHintState
{
  private const string KEY_VIEWED = "bloom_hint_viewed";
  private const string KEY_UNLOCKED = "bloom_hint_unlocked";
  private const string KEY_WELCOME = "bloom_hint_welcome_shown";
  private const char DELIM = '|';

  private readonly HashSet<string> _viewed;
  private readonly HashSet<string> _unlocked;

  public HintState()
  {
    _viewed = LoadSet(KEY_VIEWED);
    _unlocked = LoadSet(KEY_UNLOCKED);
  }

  public bool IsViewed(string id) => _viewed.Contains(id);

  public void MarkViewed(string id)
  {
    if (_viewed.Add(id))
      SaveSet(KEY_VIEWED, _viewed);
  }

  public bool IsUnlocked(string id) => _unlocked.Contains(id);

  public void Unlock(string id)
  {
    if (_unlocked.Add(id))
      SaveSet(KEY_UNLOCKED, _unlocked);
  }

  public bool IsWelcomeShown => PlayerPrefs.GetInt(KEY_WELCOME, 0) == 1;

  public void MarkWelcomeShown()
  {
    PlayerPrefs.SetInt(KEY_WELCOME, 1);
    PlayerPrefs.Save();
  }

  // debug / editor — reset ทั้งหมด เพื่อทดสอบ flow ซ้ำ
  public void ResetAll()
  {
    _viewed.Clear();
    _unlocked.Clear();
    PlayerPrefs.DeleteKey(KEY_VIEWED);
    PlayerPrefs.DeleteKey(KEY_UNLOCKED);
    PlayerPrefs.DeleteKey(KEY_WELCOME);
    PlayerPrefs.Save();
  }

  private static HashSet<string> LoadSet(string key)
  {
    var raw = PlayerPrefs.GetString(key, string.Empty);
    if (string.IsNullOrEmpty(raw)) return new HashSet<string>();
    return new HashSet<string>(raw.Split(DELIM));
  }

  private static void SaveSet(string key, HashSet<string> set)
  {
    PlayerPrefs.SetString(key, string.Join(DELIM.ToString(), set));
    PlayerPrefs.Save();
  }
}
