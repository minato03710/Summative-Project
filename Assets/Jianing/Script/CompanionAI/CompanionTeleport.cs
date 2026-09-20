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

    private float teleportTimer;

    void Start()
    {
        movement =
            GetComponent<CompanionMovement>();

        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player =
                playerObject.transform;
        }
        else
        {
            Debug.LogWarning(
                "CompanionTeleport: 找不到 Player!"
            );
        }
    }

    void Update()
    {
        if (player == null)
            return;

        if (teleportTimer > 0f)
        {
            teleportTimer -= Time.deltaTime;
        }

        CheckTeleport();
    }

    void CheckTeleport()
    {
        float distance =
            Vector3.Distance(
                transform.position,
                player.position
            );

        if (distance < teleportDistance)
            return;

        if (teleportTimer > 0f)
            return;

        TeleportToPlayer();
    }

    void TeleportToPlayer()
    {
        // 计算玩家身后的位置
        Vector3 teleportPosition =
            player.position -
            player.forward *
            teleportBehindDistance;

        // 保持和玩家大致相同高度
        teleportPosition.y =
            player.position.y;

        CharacterController controller =
            GetComponent<CharacterController>();

        // 临时关闭 CharacterController
        if (controller != null)
        {
            controller.enabled = false;
        }

        transform.position =
            teleportPosition;

        if (controller != null)
        {
            controller.enabled = true;
        }

        // 重置移动状态
        if (movement != null)
        {
            movement.Stop();
        }

        teleportTimer =
            teleportCooldown;

        Debug.Log(
            "Companion Teleported to Player!"
        );
    }


}
