using UnityEngine;

public class CompanionAI : MonoBehaviour
{
    public enum CompanionState
    {
        Follow,
        ChaseEnemy,
        Attack
    }


[Header("Target")]
    public Transform player;

    [Header("Follow Settings")]
    public float followDistance = 2.5f;
    public float followSpeed = 4f;
    public float stoppingDistance = 1.8f;

    [Header("Enemy Detection")]
    public float enemyDetectionRange = 6f;

    [Header("Combat")]
    public float attackRange = 1.8f;
    public float attackDamage = 15f;
    public float attackCooldown = 1f;

    [Header("Gravity")]
    public float gravity = -20f;

    [Header("Separation")]
    public float separationRadius = 1.2f;
    public float separationStrength = 1.5f;

    private CharacterController controller;

    private CompanionState currentState;

    private Transform currentEnemy;

    private float attackTimer;

    private float verticalVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        currentState = CompanionState.Follow;
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

        switch (currentState)
        {
            case CompanionState.Follow:
                FollowPlayer();
                break;

            case CompanionState.ChaseEnemy:
                ChaseEnemy();
                break;

            case CompanionState.Attack:
                AttackEnemy();
                break;
        }

        ApplyGravity();
    }

    // =========================
    // Follow Player
    // =========================

    void FollowPlayer()
    {
        Transform enemy = FindNearestEnemy();

        if (enemy != null)
        {
            currentEnemy = enemy;
            currentState = CompanionState.ChaseEnemy;
            return;
        }

        Vector3 direction =
            player.position -
            transform.position;

        direction.y = 0f;

        float distance =
            direction.magnitude;

        if (distance <= stoppingDistance)
        {
            return;
        }

        direction.Normalize();

        Move(direction, followSpeed);

        LookAt(direction);
    }

    // =========================
    // Chase Enemy
    // =========================

    void ChaseEnemy()
    {
        if (currentEnemy == null)
        {
            currentState = CompanionState.Follow;
            return;
        }

        EnemyHealth enemyHealth =
            currentEnemy.GetComponent<EnemyHealth>();

        if (enemyHealth == null ||
            enemyHealth.IsDead())
        {
            currentEnemy = null;
            currentState = CompanionState.Follow;
            return;
        }

        float distance =
            Vector3.Distance(
                transform.position,
                currentEnemy.position
            );

        if (distance <= attackRange)
        {
            currentState = CompanionState.Attack;
            return;
        }

        Vector3 direction =
            currentEnemy.position -
            transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.01f)
            return;

        direction.Normalize();

        Move(direction, followSpeed);

        LookAt(direction);
    }

    // =========================
    // Attack Enemy
    // =========================

    void AttackEnemy()
    {
        if (currentEnemy == null)
        {
            currentState = CompanionState.Follow;
            return;
        }

        EnemyHealth enemyHealth =
            currentEnemy.GetComponent<EnemyHealth>();

        if (enemyHealth == null ||
            enemyHealth.IsDead())
        {
            currentEnemy = null;
            currentState = CompanionState.Follow;
            return;
        }

        float distance =
            Vector3.Distance(
                transform.position,
                currentEnemy.position
            );

        if (distance > attackRange + 0.3f)
        {
            currentState = CompanionState.ChaseEnemy;
            return;
        }

        Vector3 direction =
            currentEnemy.position -
            transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
            direction.Normalize();
            LookAt(direction);
        }

        if (attackTimer > 0f)
            return;

        enemyHealth.TakeDamage(attackDamage);

        Debug.Log(
            "Companion attacked " +
            currentEnemy.name
        );

        attackTimer = attackCooldown;
    }

    // =========================
    // Find Enemy
    // =========================

    Transform FindNearestEnemy()
    {
        Collider[] enemies =
            Physics.OverlapSphere(
                transform.position,
                enemyDetectionRange
            );

        Transform nearestEnemy = null;

        float nearestDistance =
            Mathf.Infinity;

        foreach (Collider enemy in enemies)
        {
            if (!enemy.CompareTag("Enemy"))
                continue;

            EnemyHealth enemyHealth =
                enemy.GetComponent<EnemyHealth>();

            if (enemyHealth == null ||
                enemyHealth.IsDead())
                continue;

            float distance =
                Vector3.Distance(
                    transform.position,
                    enemy.transform.position
                );

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy.transform;
            }
        }

        return nearestEnemy;
    }

    // =========================
    // Movement
    // =========================

    void Move(
        Vector3 direction,
        float speed
    )
    {
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
            speed *
            Time.deltaTime
        );
    }

    // =========================
    // Enemy Separation
    // =========================

    Vector3 CalculateSeparation()
    {
        Vector3 separation =
            Vector3.zero;

        Collider[] nearbyObjects =
            Physics.OverlapSphere(
                transform.position,
                separationRadius
            );

        foreach (Collider other in nearbyObjects)
        {
            if (other.gameObject == gameObject)
                continue;

            if (!other.CompareTag("Enemy"))
                continue;

            Vector3 difference =
                transform.position -
                other.transform.position;

            difference.y = 0f;

            float distance =
                difference.magnitude;

            if (distance <= 0.01f)
                continue;

            float strength =
                1f -
                Mathf.Clamp01(
                    distance /
                    separationRadius
                );

            separation +=
                difference.normalized *
                strength;
        }

        return separation;
    }

    // =========================
    // Look Direction
    // =========================

    void LookAt(Vector3 direction)
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
                10f *
                Time.deltaTime
            );
    }

    // =========================
    // Gravity
    // =========================

    void ApplyGravity()
    {
        if (controller == null)
            return;

        if (controller.isGrounded)
        {
            if (verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }
        }
        else
        {
            verticalVelocity +=
                gravity *
                Time.deltaTime;
        }

        controller.Move(
            Vector3.up *
            verticalVelocity *
            Time.deltaTime
        );
    }

    // =========================
    // Debug
    // =========================

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            enemyDetectionRange
        );

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            attackRange
        );
    }


}

