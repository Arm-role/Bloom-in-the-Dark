using UnityEngine;
using UnityEngine.U2D.IK; // ต้องใช้ namespace นี้สำหรับ 2D IK

public class FootGrounder2D : MonoBehaviour
{
    [Header("Left Foot IK")]
    // ลาก Effector Target ของเท้าซ้าย (น่าจะเป็น Transform ที่ควบคุม bone_11)
    public Transform leftFootEffectorTarget;
    // ลาก LimbSolver2D ของเท้าซ้าย (New LimbSolver2D)
    public LimbSolver2D leftFootSolver;

    [Header("Right Foot IK")]
    // ลาก Effector Target ของเท้าขวา (น่าจะเป็น Transform ที่ควบคุม bone_12)
    public Transform rightFootEffectorTarget;
    // ลาก LimbSolver2D ของเท้าขวา (New LimbSolver2D (1))
    public LimbSolver2D rightFootSolver;

    [Header("Ground Detection Settings")]
    public float raycastDistance = 0.5f; // ระยะทางที่ยิง Ray ลงไป (ปรับให้เหมาะสมกับขนาดตัวละคร)
    public LayerMask groundLayer; // กำหนด Layer ของพื้นผิว เช่น "Ground"

    void LateUpdate()
    {
        // ทำงานกับเท้าซ้าย
        if (leftFootSolver != null && leftFootEffectorTarget != null)
        {
            AdjustFootIK(leftFootSolver, leftFootEffectorTarget);
        }

        // ทำงานกับเท้าขวา
        if (rightFootSolver != null && rightFootEffectorTarget != null)
        {
            AdjustFootIK(rightFootSolver, rightFootEffectorTarget);
        }
    }

    void AdjustFootIK(LimbSolver2D solver, Transform target)
    {
        // 1. กำหนดจุดเริ่มต้นของ Raycast ให้สูงกว่าเท้าเล็กน้อยเพื่อความแม่นยำ
        Vector3 startPos = target.position + Vector3.up * 0.1f;

        // 2. ยิง Raycast ลงไปด้านล่าง
        RaycastHit2D hit = Physics2D.Raycast(startPos, Vector2.down, raycastDistance, groundLayer);

        // Debug.DrawRay(startPos, Vector2.down * raycastDistance, hit.collider != null ? Color.green : Color.red); // เปิดดู Raycast ใน Scene View

        if (hit.collider != null)
        {
            // ถ้าชนพื้น:
            // 1. ตั้งน้ำหนัก IK เป็น 1 เพื่อให้ IK ควบคุมเต็มที่
            solver.weight = 1f;

            // 2. ปรับตำแหน่ง Effector Target ไปที่จุดชนบนพื้น
            // (รักษาค่า X ของ Target เดิม แต่เปลี่ยนค่า Y เป็นตำแหน่งของพื้น)
            target.position = new Vector3(target.position.x, hit.point.y, target.position.z);

            // *สำหรับการวางเท้าบนพื้นเอียง:* // คุณสามารถปรับ Rotation ของ Target ให้ตรงกับ normal ของพื้นได้ด้วย (ยากขึ้น)
            // target.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
        }
        else
        {
            // ถ้าไม่ชนพื้น (เท้าอยู่ในอากาศ):
            // 1. ตั้งน้ำหนัก IK เป็น 0 
            solver.weight = 0f;

            // 2. ปล่อยให้แอนิเมชันเดิมกำหนดตำแหน่งเท้า (IK จะหยุดทำงาน)
        }
    }
}