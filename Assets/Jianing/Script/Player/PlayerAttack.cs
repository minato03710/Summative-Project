using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackDamage = 25f;
    public float attackRange = 2f;
    public float attackRadius = 1.2f;
    public float attackCooldown = 0.5f;

    [Header("Layers")]
    public LayerMask enemyLayer;

    private float cooldownTimer;

    void Update()
    {
        // Attack cooldown
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        // Left-click attack
        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryAttack();
        }
    }

    void TryAttack()
    {
        // Cannot attack while cooling down
        if (cooldownTimer > 0)
            return;

        Attack();

        cooldownTimer = attackCooldown;
    }

    void Attack()
    {
        Debug.Log("Player Attack!");

        // Attack the center point
        Vector3 attackCenter =
            transform.position +
            transform.forward * attackRange;

        // Enemies within detection range
        Collider[] hitEnemies = Physics.OverlapSphere(
            attackCenter,
            attackRadius,
            enemyLayer
        );

        foreach (Collider enemy in hitEnemies)
        {
            EnemyHealth enemyHealth =
                enemy.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);

                Debug.Log("Enemy Hit!");
            }
        }
    }

    // Display attack range in the Scene window
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Vector3 attackCenter =
            transform.position +
            transform.forward * attackRange;

        Gizmos.DrawWireSphere(
            attackCenter,
            attackRadius
        );
    }
}


