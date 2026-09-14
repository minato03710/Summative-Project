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
            AttackPlayer();
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

        LookAtPlayer(direction);
    }

    void AttackPlayer()
    {
        Vector3 direction =
            player.position -
            transform.position;

        LookAtPlayer(direction);

        if (attackTimer > 0f)
            return;

        FireBullet();

        attackTimer =
            attackCooldown;
    }

    void FireBullet()
    {
        if (bulletPrefab == null)
        {
            Debug.LogWarning(
                gameObject.name +
                " 没有设置 Bullet Prefab!"
            );

            return;
        }

        if (attackPoint == null)
        {
            Debug.LogWarning(
                gameObject.name +
                " 没有设置 Attack Point!"
            );

            return;
        }

        Vector3 direction =
            player.position -
            attackPoint.position;

        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.01f)
            return;

        direction.Normalize();

        GameObject bullet =
            Instantiate(
                bulletPrefab,
                attackPoint.position,
                Quaternion.LookRotation(
                    direction
                )
            );

        EnemyBullet enemyBullet =
            bullet.GetComponent<EnemyBullet>();

        if (enemyBullet != null)
        {
            enemyBullet.SetDirection(
                direction
            );

            enemyBullet.damage =
                attackDamage;
        }
    }

    void LookAtPlayer(Vector3 direction)
    {
        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.01f)
            return;

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


}





