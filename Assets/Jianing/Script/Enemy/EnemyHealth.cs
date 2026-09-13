using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;

    private float currentHealth;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;

        currentHealth = Mathf.Max(
            currentHealth,
            0
        );

        Debug.Log(
            gameObject.name +
            " HP: " +
            currentHealth
        );

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead)
            return;

        isDead = true;

        Debug.Log(
            gameObject.name +
            " Died!"
        );

        // 获取随机掉落系统
        EnemyDropSystem dropSystem =
            GetComponent<EnemyDropSystem>();

        // 如果存在掉落系统，就生成资源
        if (dropSystem != null)
        {
            dropSystem.DropResources();
        }

        // 销毁敌人
        Destroy(gameObject);
    }

    public float GetCurrentHealth()
    {
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







