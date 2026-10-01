using UnityEngine;

public class RedLightDetector : MonoBehaviour
{
    [Header("Detection Parameters")]
    [SerializeField] private float movementThreshold = 0.1f;
    [SerializeField] private string zoneTag = "RedLightZone";

    private CharacterController characterController;
    private bool isInZone = false;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        isInZone = false;
    }

    private void Update()
    {
        // 1. Only check if player is inside the designated zone
        if (!isInZone) return;

        // 2. Only check if traffic light is RED
        if (RedLightGreenLightManager.Instance == null) return;
        if (RedLightGreenLightManager.Instance.CurrentState != RedLightGreenLightManager.LightState.Red) return;

        // 3. Check for movement input (WASD / Stick)
        bool hasInput = Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.01f || 
                        Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.01f;

        // 4. Check physical velocity on CharacterController
        bool isMoving = characterController != null && 
                        characterController.velocity.sqrMagnitude > (movementThreshold * movementThreshold);

        if (hasInput || isMoving)
        {
            OnCaughtMoving();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(zoneTag))
        {
            isInZone = true;
            Debug.Log("[RedLightDetector] Entered Red Light Zone.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(zoneTag))
        {
            isInZone = false;
            Debug.Log("[RedLightDetector] Exited Red Light Zone.");
        }
    }

    private void OnCaughtMoving()
    {
        Debug.LogWarning("[RedLightDetector] CAUGHT MOVING ON RED!");

        // Reset zone status immediately before teleporting to prevent physics glitch
        isInZone = false;

        GameObject spawnPoint = GameObject.FindWithTag("SpawnPoint");
        if (spawnPoint != null)
        {
            if (characterController != null) characterController.enabled = false;
            transform.position = spawnPoint.transform.position;
            if (characterController != null) characterController.enabled = true;
        }
    }
}