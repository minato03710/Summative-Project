using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Door Reaction")]
    [SerializeField] private bool destroyOnUnlock = false;
    [SerializeField] private Vector3 slideOffset = new Vector3(0, -10f, 0); // Direction & distance to move
    [SerializeField] private float openSpeed = 5f;

    private bool isOpen = false;
    private Vector3 targetPosition;

    [ContextMenu("Test Open Door")]
    public void TestOpenDoor()
    {
        OpenDoor();
    }

    public void OpenDoor()
    {
        if (isOpen) return;

        isOpen = true;

        // Calculate target position at the exact moment the door opens
        targetPosition = transform.position + slideOffset;

        Collider col = GetComponent<Collider>();
        if (col != null && destroyOnUnlock)
        {
            col.enabled = false;
        }

        if (destroyOnUnlock)
        {
            Destroy(gameObject, 0.5f);
        }
    }

    private void Update()
    {
        if (isOpen && !destroyOnUnlock)
        {
            // Smooth, linear movement to target position
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, openSpeed * Time.deltaTime);
        }
    }
}