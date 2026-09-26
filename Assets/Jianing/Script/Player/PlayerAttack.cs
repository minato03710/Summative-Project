using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackDamage = 25f;
    public float baseAttackDamage = 25f;

    public float attackRange = 2f;
    public float attackRadius = 1.2f;

    public float attackCooldown = 0.5f;
    public float baseAttackCooldown = 0.5f;

    [Header("Charged Attack")]
    [Min(0.01f)]
    public float chargeDuration = 2f;

    [Min(1f)]
    public float chargedDamageMultiplier = 3f;

    [Header("Layers")]
    public LayerMask enemyLayer;

    [Header("Mouse Attack")]
    public float rotationSpeed = 15f;

    private Camera mainCamera;
    private PlayerHealth playerHealth;

    private float cooldownTimer;

    private bool isCharging;
    private double chargeStartedAt;

    private bool statsInitialized;

    private float weaponDamageMultiplier = 1f;
    private float weaponAttackCooldown = -1f;

    private readonly HashSet<EnemyHealth> damagedEnemies =
        new HashSet<EnemyHealth>();

    public bool IsCharging => isCharging;

    public float ChargeProgress
    {
        get
        {
            if (!isCharging)
                return 0f;

            float elapsed =
                (float)(Time.timeAsDouble - chargeStartedAt);

            return Mathf.Clamp01(
                elapsed / Mathf.Max(0.01f, chargeDuration)
            );
        }
    }

    public bool IsFullyCharged =>
        isCharging && ChargeProgress >= 1f;

    private void Awake()
    {
        EnsureStatsInitialized();

        playerHealth = GetComponent<PlayerHealth>();
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        Mouse mouse = Mouse.current;

        // 暂停、鼠标不可用或玩家死亡时取消蓄力。
        if (Time.timeScale <= 0f ||
            mouse == null ||
            (playerHealth != null && playerHealth.IsDead()))
        {
            CancelCharge();
            return;
        }

        cooldownTimer =
            Mathf.Max(0f, cooldownTimer - Time.deltaTime);

        // 左键按下：开始蓄力，暂时不造成伤害。
        if (mouse.leftButton.wasPressedThisFrame &&
            cooldownTimer <= 0f &&
            !PointerOverUI())
        {
            isCharging = true;
            chargeStartedAt = Time.timeAsDouble;
        }

        if (!isCharging)
            return;

        // 鼠标进入可交互 UI 时取消，避免点击界面触发攻击。
        if (PointerOverUI())
        {
            CancelCharge();
            return;
        }

        // 左键松开：根据蓄力时间决定普通攻击或蓄力攻击。
        if (mouse.leftButton.wasReleasedThisFrame)
        {
            bool charged = IsFullyCharged;

            CancelCharge();
            ReleaseAttack(charged);
        }
        else if (!mouse.leftButton.isPressed)
        {
            CancelCharge();
        }
    }

    private bool PointerOverUI()
    {
        return EventSystem.current != null &&
               EventSystem.current.IsPointerOverGameObject();
    }

    private void ReleaseAttack(bool charged)
    {
        if (cooldownTimer > 0f)
            return;

        if (!GetMouseDirection(out Vector3 direction))
            return;

        RotatePlayer(direction);

        float damage = attackDamage;

        if (charged)
        {
            damage *= Mathf.Max(1f, chargedDamageMultiplier);
        }

        Vector3 attackCenter =
            transform.position + direction * attackRange;

        Collider[] hits = Physics.OverlapSphere(
            attackCenter,
            attackRadius,
            enemyLayer
        );

        damagedEnemies.Clear();

        foreach (Collider hit in hits)
        {
            EnemyHealth enemy =
                hit.GetComponentInParent<EnemyHealth>();

            // 同一敌人有多个碰撞体时，也只扣一次血。
            if (enemy != null &&
                !enemy.IsDead() &&
                damagedEnemies.Add(enemy))
            {
                enemy.TakeDamage(damage);
            }
        }

        cooldownTimer = Mathf.Max(0f, attackCooldown);

        Debug.Log(
            (charged ? "Charged attack: " : "Normal attack: ") +
            damage
        );
    }

    private bool GetMouseDirection(out Vector3 direction)
    {
        direction = Vector3.zero;

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera == null || Mouse.current == null)
            return false;

        Vector2 mousePosition =
            Mouse.current.position.ReadValue();

        Ray ray =
            mainCamera.ScreenPointToRay(mousePosition);

        Plane groundPlane = new Plane(
            Vector3.up,
            transform.position
        );

        if (!groundPlane.Raycast(ray, out float distance))
            return false;

        direction =
            ray.GetPoint(distance) - transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
            return false;

        direction.Normalize();
        return true;
    }

    private void RotatePlayer(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.01f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    public void CancelCharge()
    {
        isCharging = false;
        chargeStartedAt = 0d;
    }

    private void EnsureStatsInitialized()
    {
        if (statsInitialized)
            return;

        baseAttackDamage = attackDamage;
        baseAttackCooldown = attackCooldown;

        statsInitialized = true;
    }

    public void ApplyWeaponStats(
        float damageMultiplier,
        float weaponCooldown
    )
    {
        EnsureStatsInitialized();
        CancelCharge();

        weaponDamageMultiplier =
            Mathf.Max(0.01f, damageMultiplier);

        weaponAttackCooldown =
            Mathf.Max(0.01f, weaponCooldown);

        RefreshAttackStats();
    }

    public void RemoveWeaponStats()
    {
        EnsureStatsInitialized();
        CancelCharge();

        weaponDamageMultiplier = 1f;
        weaponAttackCooldown = -1f;

        RefreshAttackStats();
    }

    public void RefreshAttackStats()
    {
        EnsureStatsInitialized();

        attackDamage =
            baseAttackDamage * weaponDamageMultiplier;

        attackCooldown =
            weaponAttackCooldown > 0f
                ? weaponAttackCooldown
                : baseAttackCooldown;
    }

    private void OnDisable()
    {
        CancelCharge();
    }

    private void OnApplicationFocus(bool focused)
    {
        if (!focused)
        {
            CancelCharge();
        }
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused)
        {
            CancelCharge();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Vector3 attackCenter =
            transform.position + transform.forward * attackRange;

        Gizmos.DrawWireSphere(
            attackCenter,
            attackRadius
        );
    }
}