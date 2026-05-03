using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour, IBarView
{
    [SerializeField] private string barName; // แดง

    [Header("UI")] 
    [SerializeField] private Image fillTop; // แดง
    [SerializeField] private Image fillBottom; // ขาว
    [SerializeField] private TextMeshProUGUI amountText; // ขาว

    [Header("Effect Settings")] 
    [SerializeField] private float damageDelaySpeed = 0.5f;
    [SerializeField] private float healDelaySpeed = 0.8f;
    public string Name => barName;

    private void Update()
    {
        float target = fillTop.fillAmount;
        float current = fillBottom.fillAmount;

        if (Mathf.Approximately(current, target))
            return;

        float speed = current > target
            ? damageDelaySpeed    // ลด
            : healDelaySpeed;     // เพิ่ม

        fillBottom.fillAmount = Mathf.MoveTowards(
            current,
            target,
            speed * Time.deltaTime
        );
    }

    // ===== VIEW API =====
    public void SetHealth(float current, float max)
    {
        //Debug.Log($"SetHealth {current}/{max}");
        if (max <= 0f)
            return;

        float normalized = Mathf.Clamp01(current / max);
        fillTop.fillAmount = normalized;

        if (amountText != null)
            amountText.text = $"{Mathf.CeilToInt(current)}/{Mathf.CeilToInt(max)}";
    }

    public void SetHealthImmediate(float current, float max)
    {
        if (max <= 0f)
            return;

        float normalized = Mathf.Clamp01(current / max);
        fillTop.fillAmount = normalized;
        fillBottom.fillAmount = normalized;

        if (amountText != null)
            amountText.text = $"{Mathf.CeilToInt(current)}/{Mathf.CeilToInt(max)}";
    }
}