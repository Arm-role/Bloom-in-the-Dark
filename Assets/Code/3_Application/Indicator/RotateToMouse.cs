using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToMouse : MonoBehaviour
{
    [SerializeField] private bool flipY; // ใช้ถ้าต้องกลับด้าน sprite
    [SerializeField] private float offsetAngle = 0f; // เพิ่มมุมชดเชย

    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        RotateToCursor();
    }

    private void RotateToCursor()
    {
        // แปลงตำแหน่งเมาส์จากจอ → world space
        Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 dir = mousePos - transform.position;
        dir.z = 0f; // ตัดค่า z ทิ้ง เพราะเราหมุนใน 2D

        // คำนวณมุมที่ต้องหมุน
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // หมุนรอบแกน z
        transform.rotation = Quaternion.Euler(55, 0, angle + offsetAngle);

        // ถ้าต้องกลับด้าน sprite ตามการหัน
        if (flipY)
        {
            Vector3 localScale = transform.localScale;
            localScale.y = (angle > 90 || angle < -90) ? -Mathf.Abs(localScale.y) : Mathf.Abs(localScale.y);
            transform.localScale = localScale;
        }
    }
}
