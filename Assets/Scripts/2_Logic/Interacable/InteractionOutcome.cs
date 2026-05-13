public enum InteractionOutcome
{
    None,       // ไม่มีใครรับ intent นี้
    Passed,     // รับแล้ว แต่ไม่ทำอะไร
    Consumed,   // ทำสำเร็จ และหยุด interaction
    Blocked     // ตั้งใจ block (เช่น obstacle)
}