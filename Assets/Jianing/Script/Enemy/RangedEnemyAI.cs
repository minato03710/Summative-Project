using UnityEngine;

public class RangedEnemyAI : EnemyAI
{
    [Header("Ranged Attack")]
    public float attackRange = 8f;
    public float attackDamage = 10f;
    public float attackCooldown = 2f;

    [Header("Projectile")]
    public GameObject bulletPrefab;
    public Transform attackPoint;

    private float attackTimer;

    protected override void Start()
    {
        base.Start();

        attackTimer = 0f;
    }

    protected override void Update()
    {
        if (player == null)
            return;

        // attack cooldown
        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
        }

        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        // Player is outside the detection range
        if (distance > detectionRange)
        {
            playerDetected = false;
            return;
        }

        playerDetected = true;

        // The player has not yet entered the attack range.
        if (distance > attackRange)
        {
            MoveTowardsPlayer();
        }
        else
        {
            // stop moving
            AttackPlayer();
        }
    }

    protected override void MoveTowardsPlayer()
    {
        if (player == null)
            return;

        Vector3 direction =
            player.position - transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.01f)
            return;

        direction.Normalize();

        transform.position +=
            direction * moveSpeed * Time.deltaTime;

        LookAtPlayer();
    }

    void AttackPlayer()
    {
        // Facing player
        LookAtPlayer();

        // cooldown
        if (attackTimer > 0f)
            return;

        FireBullet();

        // Reset attack cooldown
        attackTimer = attackCooldown;
    }

    void FireBullet()
    {
        if (bulletPrefab == null)
        {
            Debug.LogWarning(
                gameObject.name +
                " No Bullet Prefab!"
            );

            return;
        }

        if (attackPoint == null)
        {
            Debug.LogWarning(
                gameObject.name +
                " No Attack Point!"
            );

            return;
        }

        Vector3 direction =
            player.position - attackPoint.position;

        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.01f)
            return;

        direction.Normalize();

        GameObject bullet = Instantiate(
            bulletPrefab,
            attackPoint.position,
            Quaternion.LookRotation(direction)
        );

        EnemyBullet enemyBullet =
            bullet.GetComponent<EnemyBullet>();

        if (enemyBullet != null)
        {
            enemyBullet.SetDirection(direction);
            enemyBullet.damage = attackDamage;
        }

        Debug.Log(
            gameObject.name +
            " fired a bullet!"
        );
    }

    void LookAtPlayer()
    {
        if (player == null)
            return;

        Vector3 direction =
            player.position - transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.01f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            10f * Time.deltaTime
        );
    }
}




