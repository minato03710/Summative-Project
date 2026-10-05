using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    [Header("Fracture Settings")]
    [SerializeField] private GameObject fracturedPrefab;
    [SerializeField] private float explosionForce = 500f;
    [SerializeField] private float explosionRadius = 2f;
    [SerializeField] private float pieceLifetime = 4f; // Time before pieces disappear

    [Header("Optional Directional Impact")]
    [SerializeField] private bool useImpactDirection = false;
    
    [ContextMenu("Test Break")]
public void TestBreak()
{
    Break(transform.position);
}

    // Call this method when the player hits/destroys the object
    public void Break(Vector3 hitPoint)
    {
        // 1. Spawn the fractured model at exact position/rotation of intact object
        GameObject fracturedInstance = Instantiate(fracturedPrefab, transform.position, transform.rotation);

        // 2. Apply physics force to all fractured pieces
        Rigidbody[] pieceRigidbodies = fracturedInstance.GetComponentsInChildren<Rigidbody>();
        
        foreach (Rigidbody rb in pieceRigidbodies)
        {
            if (useImpactDirection)
            {
                // Push pieces away from the hit point
                Vector3 direction = (rb.transform.position - hitPoint).normalized;
                rb.AddForce(direction * (explosionForce * 0.1f), ForceMode.Impulse);
            }
            else
            {
                // Standard omnidirectional explosion outward
                rb.AddExplosionForce(explosionForce, hitPoint, explosionRadius);
            }
        }

        // 3. Cleanup fractured pieces after a delay to maintain performance
        Destroy(fracturedInstance, pieceLifetime);

        // 4. Destroy the intact original object
        Destroy(gameObject);
    }

    // Test trigger: Pressing 'E' or colliding with player can trigger Break
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Break(other.transform.position);
        }
    }
}