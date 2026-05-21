# <ชื่อระบบ>

> Template สำหรับเอกสารระบบ — copy ไฟล์นี้แล้วแก้ ชื่อไฟล์ใช้ kebab-case (เช่น `flow-field.md`)

## Entry point

`<ไฟล์>.cs` → `<Method>()` — จุดเริ่ม trace 1 บรรทัด

## Files

| symbol / class | path | หน้าที่ |
|----------------|------|---------|
| `ClassA` | `3_Application/.../ClassA.cs` | ... |

## Flow

```
A.Method()
  → B.Method()
      → C.Method()
```

## Contracts (public API — เปลี่ยนช้า เชื่อถือได้)

- `ClassA`: `event X`, `Method()` — ...

## Gotchas

- ...

<!--
กฎ:
- Anchor ด้วยชื่อ symbol เท่านั้น ห้ามใส่ line number (มันเลื่อนทุกครั้งที่แก้โค้ด)
- เขียน contract + flow ไม่ต้องเขียน implementation ภายใน
- แก้ระบบเมื่อไหร่ → อัปเดตไฟล์นี้รอบเดียวกัน
-->
