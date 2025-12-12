using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour, IHealthBarView
{
    [SerializeField] private string barName;     // แดง

    [Header("UI")]
    [SerializeField] private Image fillTop;     // แดง
    [SerializeField] private Image fillBottom;  // ขาว

    [Header("Effect Settings")]
    [SerializeField] private float delaySpeed = 0.5f;

    private float maxHP;
    private float currentHP;

    public string Name => barName;

    public void Setup(float hp)
    {
        maxHP = hp;
        currentHP = hp;
        UpdateUIImmediate();
    }

    private void Update()
    {
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

    // ====== LOGIC ======

    public void TakeDamage(float dmg)
    {
        currentHP = Mathf.Clamp(currentHP - dmg, 0, maxHP);
        UpdateTop();
    }

    public void Heal(float amount)
    {
        currentHP = Mathf.Clamp(currentHP + amount, 0, maxHP);
        UpdateTop();
    }

    // ====== UI ======

    private void UpdateTop()
    {
        fillTop.fillAmount = currentHP / maxHP;
    }

    private void UpdateUIImmediate()
    {
        float value = currentHP / maxHP;
        fillTop.fillAmount = value;
        fillBottom.fillAmount = value;
    }
}