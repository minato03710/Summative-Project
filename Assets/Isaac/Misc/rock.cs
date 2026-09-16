using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    [Header("Wall Settings")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private GameObject breakParticleEffect; // Optional: dust or stone particle VFX

    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    // Call this method from your teammate's attack script
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            Break();
        }
    }

    private void Break()
    {
        // Optional: Spawn a dust/rock particle effect where the wall was before destroying it
        if (breakParticleEffect != null)
        {
            Instantiate(breakParticleEffect, transform.position, Quaternion.identity);
        }

        // Destroys this rock GameObject instantly from the scene
        Destroy(gameObject);
    }
}