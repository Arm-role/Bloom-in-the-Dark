using System;
using UnityEngine;

[Serializable]
public class WaveDefinition
{
  [Header("Scaling Curves (x = day, y = value)")]
  [Tooltip("จำนวน enemy ทั้งหมดที่จะ spawn ใน wave นี้ ตาม day")]
  public AnimationCurve poolCountByDay = AnimationCurve.Linear(1f, 30f, 100f, 200f);

  [Tooltip("ตัวคูณ HP ของ enemy ตาม day (1.0 = baseline)")]
  public AnimationCurve hpMultiplierByDay = AnimationCurve.Linear(1f, 1f, 100f, 5f);

  [Tooltip("ตัวคูณ damage ของ enemy ตาม day (1.0 = baseline)")]
  public AnimationCurve damageMultiplierByDay = AnimationCurve.Linear(1f, 1f, 100f, 3f);

  [Header("Spawn Pattern")]
  public SpawnPattern spawn;

  [Header("Boss")]
  [Tooltip("ปล่อยว่าง = ไม่มี boss")]
  public ObjectKey bossEnemy;

  [Tooltip("Boss spawn ทุก N day (Day N, 2N, 3N, ...). 0 = ปิด boss")]
  public int bossDayInterval = 50;

  [Tooltip("ตัวคูณ HP boss ตาม day")]
  public AnimationCurve bossHpMultiplierByDay = AnimationCurve.Linear(50f, 1f, 200f, 4f);

  [Tooltip("ตัวคูณ damage boss ตาม day")]
  public AnimationCurve bossDamageMultiplierByDay = AnimationCurve.Linear(50f, 1f, 200f, 3f);
}
