using UnityEngine;

public class RedLightDetector : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string respawnTag = "RespawnPoint";
    [SerializeField] private string fallbackSpawnTag = "SpawnPoint";
    [SerializeField] private float movementThreshold = 0.1f;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            if (RedLightGreenLightManager.Instance != null && 
                RedLightGreenLightManager.Instance.CurrentState == RedLightGreenLightManager.LightState.Red)
            {
                bool isMoving = false;

                CharacterController cc = other.GetComponent<CharacterController>();
                if (cc != null)
                {
                    isMoving = cc.velocity.sqrMagnitude > (movementThreshold * movementThreshold);
                }
                else
                {
                    Rigidbody rb = other.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        isMoving = rb.linearVelocity.sqrMagnitude > (movementThreshold * movementThreshold);
                    }
                }

                if (isMoving)
                {
                    Debug.Log("[RedLightDetector] Player caught moving during Red Light!");
                    RespawnPlayer(other.gameObject);
                }
            }
        }
    }

    private void RespawnPlayer(GameObject player)
    {
        GameObject[] respawnPoints = GameObject.FindGameObjectsWithTag(respawnTag);
        GameObject targetPoint = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 playerPos = player.transform.position;

        foreach (GameObject point in respawnPoints)
        {
            float distanceSqr = (point.transform.position - playerPos).sqrMagnitude;
            if (distanceSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqr;
                targetPoint = point;
            }
        }

        if (targetPoint == null)
        {
            targetPoint = GameObject.FindWithTag(fallbackSpawnTag);
        }

        if (targetPoint == null)
        {
            Debug.LogError($"[RedLightDetector] Neither '{respawnTag}' nor '{fallbackSpawnTag}' was found in scene!");
            return;
        }

        CharacterController playerCC = player.GetComponent<CharacterController>();
        if (playerCC != null) playerCC.enabled = false;

        player.transform.position = targetPoint.transform.position;
        player.transform.rotation = Quaternion.identity;

        if (playerCC != null) playerCC.enabled = true;

        Debug.Log($"[RedLightDetector] Player respawned at nearest point: {targetPoint.name}");
    }
}