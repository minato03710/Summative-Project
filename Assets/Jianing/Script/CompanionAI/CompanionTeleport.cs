using UnityEngine;

public class CompanionTeleport : MonoBehaviour
{
    [Header("Teleport")]
    public float teleportDistance = 12f;
    public float teleportBehindDistance = 2.5f;

    [Header("Teleport Cooldown")]
    public float teleportCooldown = 1f;

    private Transform player;
    private CompanionMovement movement;
    private CompanionCommandController commands;

    private float teleportTimer;

    private void Start()
    {
        commands = GetComponent<CompanionCommandController>();
        movement = GetComponent<CompanionMovement>();

        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning(
                "CompanionTeleport: Player not found."
            );
        }
    }

    private void Update()
    {
        if (Time.timeScale <= 0f ||
            (commands != null && commands.IsSitting))
        {
            return;
        }

        if (player == null)
            return;

        if (teleportTimer > 0f)
        {
            teleportTimer -= Time.deltaTime;
        }

        CheckTeleport();
    }

    private void CheckTeleport()
    {
        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        if (distance < teleportDistance)
            return;

        if (teleportTimer > 0f)
            return;

        TeleportToPlayer();
    }

    private void TeleportToPlayer()
    {
        Vector3 teleportPosition =
            player.position -
            player.forward * teleportBehindDistance;

        teleportPosition.y = player.position.y;

        CharacterController controller =
            GetComponent<CharacterController>();

        if (controller != null)
        {
            controller.enabled = false;
        }

        transform.position = teleportPosition;

        if (controller != null)
        {
            controller.enabled = true;
        }

        if (movement != null)
        {
            movement.Stop();
        }

        teleportTimer = teleportCooldown;
    }
}