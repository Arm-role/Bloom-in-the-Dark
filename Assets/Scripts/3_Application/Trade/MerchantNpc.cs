using UnityEngine;

// วางบน NPC พ่อค้า (spawn จาก Altar) — ตรวจ player เข้าระยะแล้วกด E เปิด trade
// ถูก wire โดย GameObjectInitializer ตอน spawn — ไม่ใช่ scene object จึงไม่ผ่าน installer
[RequireComponent(typeof(Collider2D))]
public class MerchantNpc : MonoBehaviour
{
  [SerializeField] private ShopInventory _shop;
  [Tooltip("UI prompt บอกให้กด E (optional)")]
  [SerializeField] private GameObject _interactPrompt;

  private IPlayerInput _input;
  private TradeController _tradeController;
  private bool _playerInRange;

  public void Initialize(IPlayerInput input, TradeController tradeController)
  {
    _input = input;
    _tradeController = tradeController;

    // กัน double-subscribe เผื่อ NPC ถูก spawn ซ้ำจาก pool
    _input.OnInteract -= HandleInteract;
    _input.OnInteract += HandleInteract;

    SetPrompt(false);
  }

  // เรียกโดย GameObjectInitializer ตอน despawn
  public void Teardown()
  {
    if (_input != null)
      _input.OnInteract -= HandleInteract;

    _playerInRange = false;
    SetPrompt(false);
  }

  private void OnTriggerEnter2D(Collider2D other)
  {
    if (!IsPlayer(other)) return;
    _playerInRange = true;
    SetPrompt(true);
  }

  private void OnTriggerExit2D(Collider2D other)
  {
    if (!IsPlayer(other)) return;
    _playerInRange = false;
    SetPrompt(false);
  }

  private void HandleInteract()
  {
    if (!_playerInRange || _shop == null || _tradeController == null) return;
    _tradeController.OpenTrade(_shop);
  }

  private static bool IsPlayer(Collider2D other)
  {
    var player = GlobalTargetProvider.Instance?.Player;
    if (player == null) return false;

    // collider อาจอยู่บน child ของ player → เทียบ root ด้วย
    return other.transform == player || other.transform.root == player;
  }

  private void SetPrompt(bool show)
  {
    if (_interactPrompt != null)
      _interactPrompt.SetActive(show);
  }
}
