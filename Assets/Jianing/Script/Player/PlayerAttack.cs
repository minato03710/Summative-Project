using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackDamage = 25f;


// 保存没有装备特殊武器时的攻击力
public float baseAttackDamage = 25f;

    public float attackRange = 2f;
    public float attackRadius = 1.2f;
    public float attackCooldown = 0.5f;

    // 保存没有装备特殊武器时的攻速
    public float baseAttackCooldown = 0.5f;

    [Header("Layers")]
    public LayerMask enemyLayer;

    [Header("Mouse Attack")]
    public float rotationSpeed = 15f;

    private float cooldownTimer;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        // 第一次启动时，把 Inspector 中的攻击属性作为基础值
        baseAttackDamage = attackDamage;
        baseAttackCooldown = attackCooldown;
    }

    void Update()
    {
        if (cooldownTimer > 0)
            cooldownTimer -= Time.deltaTime;

        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryAttack();
        }
    }

    void TryAttack()
    {
        if (cooldownTimer > 0)
            return;

        Vector3 attackDirection;

        if (!GetMouseDirection(out attackDirection))
            return;

        RotateTowardsMouse(attackDirection);
        Attack(attackDirection);

        cooldownTimer = attackCooldown;
    }

    bool GetMouseDirection(out Vector3 direction)
    {
        direction = Vector3.zero;

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null)
            return false;

        Vector2 mousePosition =
            Mouse.current.position.ReadValue();

        Ray ray =
            mainCamera.ScreenPointToRay(mousePosition);

        Plane groundPlane =
            new Plane(
                Vector3.up,
                new Vector3(
                    0f,
                    transform.position.y,
                    0f
                )
            );

        float distance;

        if (!groundPlane.Raycast(ray, out distance))
            return false;

        Vector3 mouseWorldPosition =
            ray.GetPoint(distance);

        direction =
            mouseWorldPosition -
            transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
            return false;

        direction.Normalize();

        return true;
    }

    void Attack(Vector3 attackDirection)
    {
        Debug.Log("Player Attack!");

        Vector3 attackCenter =
            transform.position +
            attackDirection * attackRange;

        Collider[] hitEnemies =
            Physics.OverlapSphere(
                attackCenter,
                attackRadius,
                enemyLayer
            );

        foreach (Collider enemy in hitEnemies)
        {
            EnemyHealth enemyHealth =
                enemy.GetComponent<EnemyHealth>();

            if (enemyHealth == null)
            {
                enemyHealth =
                    enemy.GetComponentInParent<EnemyHealth>();
            }

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);

                Debug.Log("Enemy Hit!");
            }
        }
    }

    void RotateTowardsMouse(Vector3 direction)
    {
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(direction);

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
    }

    public void ApplyWeaponStats(
        float damageMultiplier,
        float weaponCooldown
    )
    {
        attackDamage =
            baseAttackDamage * damageMultiplier;

        attackCooldown =
            weaponCooldown;
    }

    public void RemoveWeaponStats()
    {
        attackDamage =
            baseAttackDamage;

        attackCooldown =
            baseAttackCooldown;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Vector3 direction = transform.forward;

        Vector3 attackCenter =
            transform.position +
            direction * attackRange;

        Gizmos.DrawWireSphere(
            attackCenter,
            attackRadius
        );
    }


}
