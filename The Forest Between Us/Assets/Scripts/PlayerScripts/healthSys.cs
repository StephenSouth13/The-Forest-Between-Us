using UnityEngine;
using UnityEngine.UI;

public class healthSys : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHP = 100f;
    public float currentHP = 100f;

    [Header("UI")]
    public Image healthFillImage; // Image máu đỏ

    [Header("Effect")]
    public float smoothSpeed = 5f; // tốc độ tụt máu

    private float targetFill; // fill mong muốn

    void Start()
    {
        currentHP = maxHP;
        targetFill = 1f;
        healthFillImage.fillAmount = 1f;
    }

    void Update()
    {
        // Hiệu ứng tụt dần từ trên xuống
        healthFillImage.fillAmount = Mathf.Lerp(
            healthFillImage.fillAmount,
            targetFill,
            Time.deltaTime * smoothSpeed
        );
    }

    public void TakeDamage(float damage)
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            currentHP -= damage;
            currentHP = Mathf.Clamp(currentHP, 0, maxHP);

            targetFill = currentHP / maxHP;
        }
    }

    public void Heal(float amount)
    {
        currentHP += amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        targetFill = currentHP / maxHP;
    }
}
