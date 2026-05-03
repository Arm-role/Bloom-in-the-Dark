using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] private float maxHP = 100f;
    [SerializeField] private float currentHP;

    [Header("UI")]
    [SerializeField] private Image fillTop;     // แดง
    [SerializeField] private Image fillBottom;  // ขาว

    [Header("Effect Settings")]
    [SerializeField] private float delaySpeed = 0.5f; // ความเร็วเลือดขาวไหลลง

    private void Start()
    {
        currentHP = maxHP;
        UpdateUIImmediate();
    }

    private void Update()
    {
        // TEST ลดเลือด
        if (Input.GetKeyDown(KeyCode.LeftAlt))
            TakeDamage(10f);

        // TEST เพิ่มเลือด
        if (Input.GetKeyDown(KeyCode.H))
            Heal(10f);

        // ให้เลือดขาวค่อยๆ ลดตามแดง
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
