using UnityEngine;

public class MeleeEnemyAI : EnemyAI
{
    [Header("Melee Attack")]
    public float attackRange = 1.8f;
    public float attackDamage = 10f;
    public float attackCooldown = 1.2f;


private float attackTimer;

    protected override void Start()
    {
        base.Start();
    }

    void Update()
    {
        if (player == null)
        {
            ApplyGravity();
            return;
        }

        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
        }

        float distance =
            Vector3.Distance(
                transform.position,
                player.position
            );

        if (distance > detectionRange)
        {
            playerDetected = false;

            ApplyGravity();

            return;
        }

        playerDetected = true;

        if (distance > attackRange)
        {
            MoveTowardsPlayer();
        }
        else
        {
            Attack();
        }

        ApplyGravity();
    }

    void MoveTowardsPlayer()
    {
        Vector3 direction =
            player.position -
            transform.position;

        direction.y = 0f;

        float distance =
            direction.magnitude;

        if (distance <= stoppingDistance)
            return;

        direction.Normalize();

        Vector3 separation =
            CalculateSeparation();

        direction +=
            separation *
            separationStrength;

        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.01f)
            return;

        direction.Normalize();

        controller.Move(
            direction *
            moveSpeed *
            Time.deltaTime
        );

        Quaternion targetRotation =
            Quaternion.LookRotation(
                direction
            );

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                10f * Time.deltaTime
            );
    }

    void Attack()
    {
        if (attackTimer > 0f)
            return;

        PlayerHealth playerHealth =
            player.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(
                attackDamage
            );

            Debug.Log(
                "Melee Enemy attacked Player!"
            );
        }

        attackTimer =
            attackCooldown;
    }


}





