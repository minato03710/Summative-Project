using UnityEngine;

public class WaterHazard : MonoBehaviour
{
    [SerializeField] private int damageAmount = 10;

    private void OnTriggerEnter(Collider other)
    {
      
        if (other.CompareTag("Player"))
        {
            
            PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();

            if (playerHealth != null)
            {
            
                playerHealth.TakeDamage(damageAmount);
                
                Debug.Log($"[Water Hazard] Player entered water Subtracte {damageAmount} HP.");
            }
            else
            {
                Debug.LogWarning("[Water Hazard] Player hit water, but no PlayerHealth script was found!");
            }
        }
    }
}