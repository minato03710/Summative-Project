using UnityEngine;

public class CompanionCombat : MonoBehaviour
{
    [Header("Enemy Detection")]
    public float enemyDetectionRange = 6f;

    [Min(0.05f)]
    public float targetRefreshInterval = 0.2f;

    [Header("Combat")]
    public float attackRange = 1.8f;
    public float attackDamage = 15f;
    public float attackCooldown = 1f;
    public float combatMoveSpeed = 4f;

    private CompanionMovement movement;
    private CompanionCommandController commands;

    private EnemyHealth currentEnemy;

    private float attackTimer;
    private float searchTimer;

    // 基础伤害保持不变，实际攻击时再计算临时加成。
    public float EffectiveAttackDamage =>
        attackDamage *
        (commands != null ? commands.DamageMultiplier : 1f);

    private void Awake()
    {
        movement = GetComponent<CompanionMovement>();
        commands = GetComponent<CompanionCommandController>();
    }

    private void Update()
    {
        if (Time.timeScale <= 0f)
            return;

        attackTimer =
            Mathf.Max(0f, attackTimer - Time.deltaTime);

        searchTimer -= Time.deltaTime;

        if (commands != null && commands.IsSitting)
        {
            ClearTarget();
            return;
        }

        if (searchTimer <= 0f || !IsEnemyValid())
        {
            FindNearestEnemy();

            searchTimer =
                Mathf.Max(0.05f, targetRefreshInterval);
        }

        if (!IsEnemyValid())
            return;

        Vector3 direction =
            currentEnemy.transform.position - transform.position;

        float distance = direction.magnitude;

        direction.y = 0f;

        if (distance > attackRange)
        {
            if (movement != null)
            {
                movement.Move(direction, combatMoveSpeed);
            }

            return;
        }

        if (movement != null)
        {
            movement.Stop();
        }

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                10f * Time.deltaTime
            );
        }

        if (attackTimer <= 0f)
        {
            currentEnemy.TakeDamage(EffectiveAttackDamage);

            attackTimer = Mathf.Max(0.01f, attackCooldown);
        }
    }

    private void FindNearestEnemy()
    {
        currentEnemy = null;

        float nearestDistance =
            Mathf.Max(0f, enemyDetectionRange);

        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            nearestDistance
        );

        foreach (Collider hit in hits)
        {
            EnemyHealth candidate =
                hit.GetComponentInParent<EnemyHealth>();

            if (candidate == null ||
                !candidate.isActiveAndEnabled ||
                candidate.IsDead())
            {
                continue;
            }

            if (!candidate.CompareTag("Enemy"))
                continue;

            float distance = Vector3.Distance(
                transform.position,
                candidate.transform.position
            );

            if (distance <= nearestDistance)
            {
                nearestDistance = distance;
                currentEnemy = candidate;
            }
        }
    }

    private bool IsEnemyValid()
    {
        return currentEnemy != null &&
               currentEnemy.isActiveAndEnabled &&
               !currentEnemy.IsDead() &&
               Vector3.Distance(
                   transform.position,
                   currentEnemy.transform.position
               ) <= enemyDetectionRange;
    }

    public void ClearTarget()
    {
        currentEnemy = null;
        searchTimer = 0f;
    }

    public bool IsInCombat()
    {
        return enabled &&
               (commands == null || !commands.IsSitting) &&
               IsEnemyValid();
    }

    public Transform GetCurrentEnemy()
    {
        return IsInCombat() ? currentEnemy.transform : null;
    }
}