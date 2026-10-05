using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    [Min(1f)] public float maxHealth = 100f;

    [Header("Second Life - enabled automatically by Boss AI")]
    public bool useSecondLife;
    [Min(0f)] public float reviveProtectionSeconds = 1f;

    public event Action SecondLifeStarted;

    public bool IsSecondLife { get; private set; }

    public bool IsReviving =>
        Time.timeAsDouble < protectedUntil;

    private float currentHealth;
    private bool initialized;
    private bool isDead;
    private double protectedUntil;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (initialized) return;

        currentHealth = Mathf.Max(1f, maxHealth);
        initialized = true;
    }

    public void TakeDamage(float damage)
    {
        Initialize();

        if (isDead || IsReviving || damage <= 0f)
            return;

        currentHealth = Mathf.Max(
            0f, currentHealth - damage);

        if (currentHealth > 0f)
            return;

        if (useSecondLife && !IsSecondLife)
        {
            IsSecondLife = true;
            currentHealth = Mathf.Max(1f, maxHealth);

            protectedUntil =
                Time.timeAsDouble +
                Mathf.Max(0f, reviveProtectionSeconds);

            // 进入第二条命时清除已有中毒。
            EnemyPoison poison = GetComponent<EnemyPoison>();

            if (poison != null)
                poison.enabled = false;

            SecondLifeStarted?.Invoke();
            return;
        }

        isDead = true;

        EnemyDropSystem drops =
            GetComponent<EnemyDropSystem>();

        if (drops != null)
            drops.DropResources();

        Destroy(gameObject);
    }

    public float GetCurrentHealth()
    {
        Initialize();
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public bool IsDead()
    {
        return isDead;
    }
}