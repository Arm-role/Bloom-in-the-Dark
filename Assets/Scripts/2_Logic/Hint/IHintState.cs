#nullable enable

// Persistent state — concrete impl ใน 3_Application/Hint/HintState.cs (PlayerPrefs)
// ภายหลังย้ายไประบบ save จริง (Newtonsoft.Json) ก็ implement interface เดิม caller ไม่กระทบ
public interface IHintState
{
  bool IsViewed(string id);
  void MarkViewed(string id);

  bool IsUnlocked(string id);
  void Unlock(string id);

  // First-time welcome popup — โผล่ครั้งเดียวหลังเปิดเกมครั้งแรกใน save นั้น
  bool IsWelcomeShown { get; }
  void MarkWelcomeShown();
}
