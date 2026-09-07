
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;

    private float currentHealth;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    // get hurt
    public void TakeDamage(float damage)
    {
        // Dead, no longer geting hurt
        if (isDead)
            return;

        currentHealth -= damage;

        // Prevent health from dropping below 0
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log("Player HP: " + currentHealth);

        // Check for death
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // heal
    public void Heal(float amount)
    {
        if (isDead)
            return;

        currentHealth += amount;

        // Prevent exceeding maximum health points
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        Debug.Log("Player HP: " + currentHealth);
    }

    void Die()
    {
        isDead = true;

        Debug.Log("Player Died!");

        // Temporarily disable player movement only
        PlayerMovement movement = GetComponent<PlayerMovement>();

        if (movement != null)
        {
            movement.enabled = false;
        }

        PlayerDash dash = GetComponent<PlayerDash>();

        if (dash != null)
        {
            dash.enabled = false;
        }
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

