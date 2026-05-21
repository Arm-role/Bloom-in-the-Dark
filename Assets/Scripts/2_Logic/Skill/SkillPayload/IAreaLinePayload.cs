// capability interface — AreaLine targeting รับ payload ตัวไหนก็ได้ที่ให้ข้อมูลเส้นครบ
// (AreaLineConfigProvider depend อันนี้ ไม่ใช่ concrete payload — เพิ่ม skill เส้นใหม่ได้โดยไม่แก้ provider)
public interface IAreaLinePayload : ISkillDataPayload
{
  float Range { get; }
  float Width { get; }
  bool IsValid { get; }
}
