using UnityEngine;

public class MeleeEnemyAI : EnemyAI
{
    [Header("Melee Attack")]
    public float attackRange = 1.8f;
    public float attackDamage = 10f;
    public float attackCooldown = 1.2f;

    private float attackTimer;

    private CharacterController controller;

    protected override void Start()
    {
        base.Start();

        controller = GetComponent<CharacterController>();
    }

    protected override void Update()
    {
        if (player == null)
            return;

        // attack cooldown
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        // Player is not within detection range
        if (distance > detectionRange)
        {
            playerDetected = false;
            return;
        }

        playerDetected = true;

        // Not yet within attack range
        if (distance > attackRange)
        {
            MoveTowardsPlayer();
        }
        else
        {
            // stop and attack
            Attack();
        }
    }

    protected override void MoveTowardsPlayer()
    {
        if (player == null)
            return;

        Vector3 direction =
            player.position - transform.position;

        direction.y = 0;

        if (direction.sqrMagnitude <= 0.01f)
            return;

        direction.Normalize();

        controller.Move(
            direction * moveSpeed * Time.deltaTime
        );

        // Faceing player
        Quaternion targetRotation =
            Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            10f * Time.deltaTime
        );
    }

    void Attack()
    {
        if (attackTimer > 0)
            return;

        PlayerHealth playerHealth =
            player.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);

            Debug.Log("Melee Enemy attacked Player!");
        }

        attackTimer = attackCooldown;
    }
}


