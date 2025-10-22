public enum EItemStategyType
{
    None,

    // --- Placement Strategies ---
    GridBased,          // ใช้การวางแบบกริด (เหมือนวางสิ่งปลูกสร้าง)
    ProximityCollider,  // ใช้ collider ในการตรวจระยะ (ของประชิด เช่น melee item)
    AreaCircle,         // พื้นที่วงกลม (เช่นสกิลวงกลมใน LoL/ROV)
    AreaCone,           // พื้นที่กรวยหน้า (เช่น cone attack หรือ skill shot)
    AreaLine,           // เส้นตรง (เช่น laser หรือ projectile แบบเส้น)
    AreaBox,            // พื้นที่สี่เหลี่ยม (เช่น trap zone หรือ block zone)

    // --- Targeting / Usage ---
    SingleTarget,       // ใช้กับเป้าหมายเดี่ยว
    MultiTarget,        // ใช้กับหลายเป้าหมายในรัศมี
}