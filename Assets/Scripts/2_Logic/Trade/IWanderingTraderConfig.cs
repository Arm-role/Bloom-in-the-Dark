using UnityEngine;

// Contract สำหรับ wandering trader config — 3_Application ใช้ผ่าน interface (4_Infrastructure อ้างไม่ได้)
// Concrete impl: WanderingTraderConfig (4_Infrastructure/Trade/)
public interface IWanderingTraderConfig
{
  int NpcId { get; }

  int FirstVisitDay { get; }
  int IntervalDays { get; }
  int StayCycles { get; }

  float MinRadius { get; }
  float MaxRadius { get; }
  float CameraMargin { get; }
  float SpawnClearRadius { get; }
  LayerMask ObstacleMask { get; }
  int MaxSpawnAttempts { get; }

  float ApproachDistance { get; }
  float ArrivalDistance { get; }
}
