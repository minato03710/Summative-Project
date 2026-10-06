using UnityEngine;

public class PowerCell : MonoBehaviour
{
    [Header("Cell Durability")]
    [SerializeField] private int maxHealth = 1;
    [SerializeField] private string attackTag = "PlayerAttack"; // Tag used on weapon/projectile hitboxes

    [Header("Fracture Settings")]
    [SerializeField] private GameObject fracturedPrefab;
    [SerializeField] private float explosionForce = 300f;

    private int currentHealth;
    private bool isDestroyed = false;

    private void Start()
    {
        currentHealth = maxHealth;

        // Automatically register cell with manager
        if (PowerCellManager.Instance != null)
        {
            PowerCellManager.Instance.RegisterCell();
        }
    }

    /// <summary>
    /// Call this function from your Player's attack script, projectile, or raycast.
    /// </summary>
    public void TakeDamage(int damage = 1)
    {
        if (isDestroyed) return;

        currentHealth -= damage;
        Debug.Log($"[PowerCell] Hit! Remaining health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Break();
        }
    }

    public void Break()
    {
        if (isDestroyed) return;
        isDestroyed = true;

        // Notify Manager
        if (PowerCellManager.Instance != null)
        {
            PowerCellManager.Instance.OnCellDestroyed();
        }

        // Spawn fractured pieces
        if (fracturedPrefab != null)
        {
            GameObject frac = Instantiate(fracturedPrefab, transform.position, transform.rotation);
            foreach (Rigidbody rb in frac.GetComponentsInChildren<Rigidbody>())
            {
                rb.AddExplosionForce(explosionForce, transform.position, 3f);
            }
            Destroy(frac, 4f);
        }

        Destroy(gameObject);
    }

    // Triggered automatically if a weapon/attack hitbox with 'attackTag' touches this cell
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(attackTag))
        {
            TakeDamage(1);
        }
    }

    [ContextMenu("Test Break Cell")]
    public void TestBreakCell()
    {
        Break();
    }
}