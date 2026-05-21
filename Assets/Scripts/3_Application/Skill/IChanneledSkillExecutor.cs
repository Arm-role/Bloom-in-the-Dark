// executor ที่ต้องผูกกับ player ระหว่างทำงาน (channeled skill เช่น beam ที่ steer ตามเมาส์ + ล็อกการเดิน)
// SkillSpawnController จะเรียก BindChannel ให้หลัง Initialize ถ้า executor implement interface นี้
// optional — executor ปกติ (AreaCircle/Line/Cone) ไม่ต้อง implement
public interface IChanneledSkillExecutor
{
  void BindChannel(IPlayerInput input, IPlayerInteractor interactor);
}
