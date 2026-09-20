using UnityEngine;

public class CompanionFollow : MonoBehaviour
{
    [Header("Follow")]
    public float stoppingDistance = 1.8f;
    public float followSpeed = 4f;


private Transform player;
    private CompanionMovement movement;
    private CompanionCombat combat;

    void Start()
    {
        movement =
            GetComponent<CompanionMovement>();

        combat =
            GetComponent<CompanionCombat>();

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
                "CompanionFollow: 找不到 Player!"
            );
        }
    }

    void Update()
    {
        if (player == null)
            return;

        // 如果正在战斗，不执行跟随
        if (combat != null &&
            combat.IsInCombat())
        {
            return;
        }

        FollowPlayer();
    }

    void FollowPlayer()
    {
        Vector3 direction =
            player.position -
            transform.position;

        direction.y = 0f;

        float distance =
            direction.magnitude;

        if (distance <= stoppingDistance)
        {
            movement.Stop();
            return;
        }

        movement.Move(
            direction,
            followSpeed
        );
    }


}


