using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour, IBarView
{
    [SerializeField] private string barName; // แดง

    [Header("UI")] [SerializeField] private Image fillTop; // แดง
    [SerializeField] private Image fillBottom; // ขาว
    [SerializeField] private TextMeshProUGUI amountText; // ขาว

    [Header("Effect Settings")] [SerializeField]
    private float delaySpeed = 0.5f;

    public string Name => barName;

    private void Update()
    {
        if (fillBottom.fillAmount == fillTop.fillAmount) return;

        if (fillBottom.fillAmount > fillTop.fillAmount)
        {
            fillBottom.fillAmount = Mathf.MoveTowards(
                fillBottom.fillAmount,
                fillTop.fillAmount,
                delaySpeed * Time.deltaTime
            );
        }
        else
        {
            fillBottom.fillAmount = fillTop.fillAmount;
        }
    }

    // ===== VIEW API =====
    public void SetHealth(float current, float max)
    {
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