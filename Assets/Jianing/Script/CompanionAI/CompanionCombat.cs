using UnityEngine;

public class CompanionCombat : MonoBehaviour
{
    [Header("Enemy Detection")]
    public float enemyDetectionRange = 6f;


[Header("Combat")]
    public float attackRange = 1.8f;
    public float attackDamage = 15f;
    public float attackCooldown = 1f;
    public float combatMoveSpeed = 4f;

    private CompanionMovement movement;

    private Transform currentEnemy;

    private float attackTimer;

    void Start()
    {
        movement =
            GetComponent<CompanionMovement>();
    }

    void Update()
    {
        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
        }

        // 没有敌人 → 寻找敌人
        if (currentEnemy == null)
        {
            FindNearestEnemy();

            if (currentEnemy == null)
            {
                return;
            }
        }

        // 检查敌人是否还有效
        if (!IsEnemyValid())
        {
            currentEnemy = null;
            return;
        }

        float distance =
            Vector3.Distance(
                transform.position,
                currentEnemy.position
            );

        // 敌人距离太远
        if (distance > enemyDetectionRange)
        {
            currentEnemy = null;
            return;
        }

        // 追击
        if (distance > attackRange)
        {
            ChaseEnemy();
        }
        else
        {
            AttackEnemy();
        }
    }

    void FindNearestEnemy()
    {
        GameObject[] enemies =
            GameObject.FindGameObjectsWithTag("Enemy");

        float closestDistance =
            enemyDetectionRange;

        Transform closestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            if (enemy == null)
                continue;

            EnemyHealth enemyHealth =
                enemy.GetComponent<EnemyHealth>();

            if (enemyHealth == null)
                continue;

            if (enemyHealth.IsDead())
                continue;

            float distance =
                Vector3.Distance(
                    transform.position,
                    enemy.transform.position
                );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy.transform;
            }
        }

        currentEnemy = closestEnemy;
    }

    void ChaseEnemy()
    {
        if (currentEnemy == null)
            return;

        Vector3 direction =
            currentEnemy.position -
            transform.position;

        direction.y = 0f;

        movement.Move(
            direction,
            combatMoveSpeed
        );
    }

    void AttackEnemy()
    {
        if (currentEnemy == null)
            return;

        Vector3 direction =
            currentEnemy.position -
            transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
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

        if (attackTimer > 0f)
            return;

        EnemyHealth enemyHealth =
            currentEnemy.GetComponent<EnemyHealth>();

        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(
                attackDamage
            );

            Debug.Log(
                "Companion attacked Enemy!"
            );
        }

        attackTimer =
            attackCooldown;
    }

    bool IsEnemyValid()
    {
        if (currentEnemy == null)
            return false;

        EnemyHealth enemyHealth =
            currentEnemy.GetComponent<EnemyHealth>();

        if (enemyHealth == null)
            return false;

        if (enemyHealth.IsDead())
            return false;

        return true;
    }

    public bool IsInCombat()
    {
        return currentEnemy != null;
    }

    public Transform GetCurrentEnemy()
    {
        return currentEnemy;
    }


}


