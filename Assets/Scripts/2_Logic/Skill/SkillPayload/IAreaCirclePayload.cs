// capability interface — AreaCircle targeting รับ payload ตัวไหนก็ได้ที่ให้ข้อมูลวงกลมครบ
// (AreaCircleConfigProvider depend อันนี้ ไม่ใช่ concrete payload — เพิ่ม skill วงกลมใหม่ได้โดยไม่แก้ provider)
public interface IAreaCirclePayload : ISkillDataPayload
{
  float Range { get; }
  float Radius { get; }
  float XAngle { get; }
  bool IsValid { get; }
}
