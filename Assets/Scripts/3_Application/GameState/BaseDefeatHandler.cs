using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseDefeatHandler : MonoBehaviour
{
  [SerializeField] private BaseBuildingController _baseBuilding;
  [SerializeField] private PlayerRespawnController _playerRespawn;
  [SerializeField] private string _gameOverSceneName = "GameOver";
  [SerializeField] private float _delayBeforeLoad = 1.5f;

  private bool _triggered;

  private void Start()
  {
    if (_baseBuilding == null)
    {
      Debug.LogWarning("[BaseDefeatHandler] _baseBuilding is not assigned");
      return;
    }

    _baseBuilding.OnBuildingDestroyed += HandleDestroyed;
  }

  private void OnDestroy()
  {
    if (_baseBuilding != null)
      _baseBuilding.OnBuildingDestroyed -= HandleDestroyed;
  }

  private void HandleDestroyed()
  {
    if (_triggered) return;
    _triggered = true;

    // altar/base พัง → ซ่อน building ทันที (OnBroken เดิมแค่เคลียร์ grid ไม่ได้ซ่อน object)
    _baseBuilding.gameObject.SetActive(false);

    // กัน player ค้างใน respawn state ถ้า base พังตอน player กำลังรอเกิด
    if (_playerRespawn != null && _playerRespawn.IsRespawning)
      _playerRespawn.CancelRespawn();

    // reset session flags กลับเป็น default — เผื่อ player กด NewGame ที่ MainMenu
    GameSession.Reset();

    StartCoroutine(DelayedLoad());
  }

  private IEnumerator DelayedLoad()
  {
    yield return new WaitForSeconds(_delayBeforeLoad);

    if (string.IsNullOrEmpty(_gameOverSceneName))
    {
      Debug.LogWarning("[BaseDefeatHandler] _gameOverSceneName is empty");
      yield break;
    }

    // คืน enemy เข้า pool ก่อนเปลี่ยน scene — pool root เป็น DontDestroyOnLoad
    // ถ้าไม่คืน instance ที่ active จะตามไปโผล่ใน GameOver scene
    EnemyManager.Instance?.DespawnAll();

    SceneManager.LoadScene(_gameOverSceneName);
  }
}
