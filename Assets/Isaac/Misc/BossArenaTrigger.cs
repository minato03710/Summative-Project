using UnityEngine;

public class BossArenaTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject forcefieldBarrier;

    [Header("Settings")]
    [SerializeField] private bool triggerOnce = true;

    private bool isLocked = false;

    private void Start()
    {
        // Start with the barrier deactivated
        if (forcefieldBarrier != null)
        {
            forcefieldBarrier.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isLocked && triggerOnce) return;

        // Ensure your Player object has the "Player" tag assigned in the Inspector
        if (other.CompareTag("Player"))
        {
            LockArena();
        }
    }

    public void LockArena()
    {
        isLocked = true;
        if (forcefieldBarrier != null)
        {
            forcefieldBarrier.SetActive(true);
        }
        Debug.Log("[BossArena] Player locked inside arena!");
    }

    public void UnlockArena()
    {
        isLocked = false;
        if (forcefieldBarrier != null)
        {
            forcefieldBarrier.SetActive(false);
        }
        Debug.Log("[BossArena] Arena unlocked!");
    }
}