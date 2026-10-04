using UnityEngine;

public class LavaHazard : MonoBehaviour
{
    [Header("Hazard Settings")]
    [SerializeField] private bool instantKill = true;
    [SerializeField] private int damageAmount = 1;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string respawnTag = "RespawnPoint";
    [SerializeField] private string fallbackSpawnTag = "SpawnPoint";

    [Header("Continuous Burn (If Instant Kill is Off)")]
    [SerializeField] private float burnInterval = 1.0f;
    private float nextDamageTime;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            if (instantKill)
            {
                RespawnPlayer(other.gameObject);
            }
            else
            {
                ApplyDamage(other.gameObject);
                nextDamageTime = Time.time + burnInterval;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!instantKill && other.CompareTag(playerTag))
        {
            if (Time.time >= nextDamageTime)
            {
                ApplyDamage(other.gameObject);
                nextDamageTime = Time.time + burnInterval;
            }
        }
    }

    private void ApplyDamage(GameObject player)
    {
        Debug.Log($"[LavaHazard] Dealt {damageAmount} damage to Player.");
       
    }

    private void RespawnPlayer(GameObject player)
    {
        // 1. Get ALL active objects tagged with the respawn tag
        GameObject[] respawnPoints = GameObject.FindGameObjectsWithTag(respawnTag);
        GameObject targetPoint = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 playerPos = player.transform.position;

        // 2. Loop through all respawn points to find the closest one
        foreach (GameObject point in respawnPoints)
        {
            float distanceSqr = (point.transform.position - playerPos).sqrMagnitude;
            if (distanceSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqr;
                targetPoint = point;
            }
        }

        // 3. Fall back to the main scene SpawnPoint if no RespawnPoint exists in the scene
        if (targetPoint == null)
        {
            targetPoint = GameObject.FindWithTag(fallbackSpawnTag);
            if (targetPoint != null)
            {
                Debug.LogWarning($"[LavaHazard] No '{respawnTag}' found! Falling back to '{fallbackSpawnTag}'.");
            }
        }

        // 4. If neither exist, log error and cancel teleport
        if (targetPoint == null)
        {
            Debug.LogError($"[LavaHazard] Neither '{respawnTag}' nor '{fallbackSpawnTag}' was found in this scene!");
            return;
        }

        // 5. Disable CharacterController temporarily to teleport cleanly
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        player.transform.position = targetPoint.transform.position;
        player.transform.rotation = Quaternion.identity; // Reset rotation to prevent sprite/movement bugs

        if (cc != null) cc.enabled = true;

        Debug.Log($"[LavaHazard] Player respawned at nearest point: {targetPoint.name}");
    }
}