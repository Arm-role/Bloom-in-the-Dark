using UnityEngine;

[CreateAssetMenu(menuName = "Trade/WanderingTraderConfig")]
public class WanderingTraderConfig : ScriptableObject, IWanderingTraderConfig
{
  [Header("NPC")]
  [Tooltip("ObjectKey ของ trader NPC prefab ที่ register ใน GameObjectLibrary — pattern เดียวกับ SkillDefinition.skillObjectKey, CycleRuntime.bossEnemy")]
  [SerializeField] private ObjectKey _npcObjectKey;

  [Header("Schedule (absolute day count)")]
  [Tooltip("Day แรกที่ trader โผล่ (เช่น 5 = ครั้งแรก Day 5)")]
  [SerializeField] private int _firstVisitDay = 5;

  [Tooltip("กี่ day ระหว่างแต่ละ visit (5 = ทุก 5 day = Day 5, 10, 15, ...)")]
  [SerializeField] private int _intervalDays = 5;

  [Tooltip("จำนวน cycle (day) ที่ trader อยู่ก่อน leave — spawn Day N → ออก Day N+stayCycles ตอน Prepare")]
  [SerializeField] private int _stayCycles = 3;

  [Header("Spawn position (radius รอบ BaseBuilding)")]
  [Tooltip("Distance ขั้นต่ำจาก BaseBuilding")]
  [SerializeField] private float _minRadius = 8f;

  [Tooltip("Distance สูงสุดจาก BaseBuilding")]
  [SerializeField] private float _maxRadius = 15f;

  [Tooltip("Camera exclusion margin (1.0 = exact viewport, 1.2 = นอก viewport 20% — กัน pop-in ตอน pan)")]
  [Range(1f, 2f)]
  [SerializeField] private float _cameraMargin = 1.2f;

  [Tooltip("Radius เช็ค obstacle รอบ candidate position")]
  [SerializeField] private float _spawnClearRadius = 0.5f;

  [Tooltip("LayerMask สำหรับ obstacle (ผนัง/ของกีดขวาง) — ใช้ pattern เดียวกับ EnemySteering.obstacleMask")]
  [SerializeField] private LayerMask _obstacleMask;

  [Tooltip("Max attempts หา valid spawn — ถ้าครบยังไม่เจอ → skip visit รอบนี้")]
  [SerializeField] private int _maxSpawnAttempts = 20;

  [Header("Movement behavior")]
  [Tooltip("Walk เข้าหา BaseBuilding หลัง spawn — หยุดก่อนถึง base ระยะ N")]
  [SerializeField] private float _approachDistance = 3f;

  [Tooltip("Distance threshold ที่ถือว่าถึง exit position (despawn ได้)")]
  [SerializeField] private float _arrivalDistance = 0.5f;

  // IWanderingTraderConfig — resolve ObjectKey → hash int (ตาม pattern SkillDefinition.SkillId)
  public int NpcId => _npcObjectKey != null ? _npcObjectKey.RuntimeTag.Hash : 0;
  public int FirstVisitDay => _firstVisitDay;
  public int IntervalDays => _intervalDays;
  public int StayCycles => _stayCycles;
  public float MinRadius => _minRadius;
  public float MaxRadius => _maxRadius;
  public float CameraMargin => _cameraMargin;
  public float SpawnClearRadius => _spawnClearRadius;
  public LayerMask ObstacleMask => _obstacleMask;
  public int MaxSpawnAttempts => _maxSpawnAttempts;
  public float ApproachDistance => _approachDistance;
  public float ArrivalDistance => _arrivalDistance;
}
