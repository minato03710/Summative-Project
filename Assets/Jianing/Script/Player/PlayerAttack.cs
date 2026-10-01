using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackDamage = 25f;
    public float baseAttackDamage = 25f;

    public float attackCooldown = 0.5f;
    public float baseAttackCooldown = 0.5f;

    [Header("Unarmed Range")]
    [FormerlySerializedAs("attackRange")]
    [Min(0)] public float baseAttackRange = 2f;

    [FormerlySerializedAs("attackRadius")]
    [Min(0.01f)] public float baseAttackRadius = 1.2f;

    public float attackRange { get; private set; }
    public float attackRadius { get; private set; }

    [Header("Charged Attack")]
    [Min(0.01f)] public float chargeDuration = 2f;
    [Min(1)] public float chargedDamageMultiplier = 3f;

    [Header("Layers")]
    public LayerMask enemyLayer;

    [Header("Mouse Attack")]
    public float rotationSpeed = 15f;

    private Camera mainCamera;
    private PlayerHealth playerHealth;
    private WeaponData weapon;

    private float cooldownTimer;
    private bool isCharging;
    private double chargeStartedAt;
    private bool statsInitialized;

    private readonly HashSet<EnemyHealth> damagedEnemies =
        new HashSet<EnemyHealth>();

    public bool IsCharging => isCharging;

    public float ChargeProgress => !isCharging
        ? 0f
        : Mathf.Clamp01(
            (float)(Time.timeAsDouble - chargeStartedAt) /
            Mathf.Max(0.01f, chargeDuration));

    public bool IsFullyCharged =>
        isCharging && ChargeProgress >= 1f;

    private void Awake()
    {
        InitializeStats();
        RefreshAttackStats();
        playerHealth = GetComponent<PlayerHealth>();
    }

    public void InitializeStats()
    {
        if (statsInitialized) return;

        // 保留项目原有的初始攻击力和攻击间隔。
        baseAttackDamage = attackDamage;
        baseAttackCooldown = attackCooldown;
        statsInitialized = true;
    }

    private void Update()
    {
        Mouse mouse = Mouse.current;

        if (Time.timeScale <= 0f ||
            mouse == null ||
            (playerHealth != null && playerHealth.IsDead()))
        {
            CancelCharge();
            return;
        }

        cooldownTimer = Mathf.Max(
            0f, cooldownTimer - Time.deltaTime);

        if (mouse.leftButton.wasPressedThisFrame &&
            cooldownTimer <= 0f &&
            !PointerOverUI())
        {
            isCharging = true;
            chargeStartedAt = Time.timeAsDouble;
        }

        if (!isCharging) return;

        if (PointerOverUI())
        {
            CancelCharge();
            return;
        }

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
        if (cooldownTimer > 0f ||
            !GetMouseDirection(out Vector3 direction))
            return;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(direction),
            rotationSpeed * Time.deltaTime);

        float damage = attackDamage *
            (charged ? Mathf.Max(1f, chargedDamageMultiplier) : 1f);

        Collider[] hits = Physics.OverlapSphere(
            transform.position + direction * attackRange,
            attackRadius,
            enemyLayer);

        damagedEnemies.Clear();

        foreach (Collider hit in hits)
        {
            EnemyHealth enemy =
                hit.GetComponentInParent<EnemyHealth>();

            if (enemy == null ||
                enemy.IsDead() ||
                !damagedEnemies.Add(enemy))
                continue;

            enemy.TakeDamage(damage);

            if (!enemy.IsDead() &&
                weapon != null &&
                weapon.poisonDamagePerSecond > 0f &&
                weapon.poisonDurationSeconds > 0)
            {
                EnemyPoison poison =
                    enemy.GetComponent<EnemyPoison>();

                if (poison == null)
                {
                    poison = enemy.gameObject
                        .AddComponent<EnemyPoison>();
                }

                poison.Apply(
                    weapon.poisonDamagePerSecond,
                    weapon.poisonDurationSeconds);
            }
        }

        cooldownTimer = Mathf.Max(0.001f, attackCooldown);
    }

    private bool GetMouseDirection(out Vector3 direction)
    {
        direction = Vector3.zero;

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null || Mouse.current == null)
            return false;

        Ray ray = mainCamera.ScreenPointToRay(
            Mouse.current.position.ReadValue());

        Plane plane = new Plane(
            Vector3.up, transform.position);

        if (!plane.Raycast(ray, out float distance))
            return false;

        direction = ray.GetPoint(distance) - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
            return false;

        direction.Normalize();
        return true;
    }

    public void ApplyWeaponStats(WeaponData data)
    {
        InitializeStats();
        CancelCharge();

        weapon = data;
        RefreshAttackStats();
    }

    public void RemoveWeaponStats()
    {
        ApplyWeaponStats(null);
    }

    public void RefreshAttackStats()
    {
        InitializeStats();

        attackDamage = baseAttackDamage *
            (weapon == null
                ? 1f
                : Mathf.Max(0f, weapon.damageMultiplier));

        attackCooldown = weapon == null
            ? Mathf.Max(0.001f, baseAttackCooldown)
            : weapon.GetAttackInterval(baseAttackCooldown);

        if (weapon != null && weapon.useCustomRange)
        {
            attackRange = Mathf.Max(
                0f, weapon.customAttackRange);

            attackRadius = Mathf.Max(
                0.01f, weapon.customAttackRadius);
        }
        else
        {
            float multiplier = weapon == null
                ? 1f
                : Mathf.Max(0.01f, weapon.rangeMultiplier);

            attackRange = Mathf.Max(
                0f,
                baseAttackRange * multiplier +
                (weapon == null
                    ? 0f
                    : Mathf.Max(0f, weapon.extraRange)));

            attackRadius = Mathf.Max(
                0.01f,
                baseAttackRadius * multiplier);
        }
    }

    public void CancelCharge()
    {
        isCharging = false;
        chargeStartedAt = 0;
    }

    private void OnDisable()
    {
        CancelCharge();
    }

    private void OnApplicationFocus(bool focused)
    {
        if (!focused) CancelCharge();
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused) CancelCharge();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        float range = Application.isPlaying
            ? attackRange
            : baseAttackRange;

        float radius = Application.isPlaying
            ? attackRadius
            : baseAttackRadius;

        Gizmos.DrawWireSphere(
            transform.position + transform.forward * range,
            radius);
    }
}