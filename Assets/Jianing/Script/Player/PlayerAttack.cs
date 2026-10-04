using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;

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

    [Header("Charged Melee Attack")]
    [Min(0.01f)] public float chargeDuration = 2f;
    [Min(1)] public float chargedDamageMultiplier = 3f;

    [Header("Layers")]
    public LayerMask enemyLayer;
    public LayerMask projectileObstacleLayers;

    [Header("Mouse Attack")]
    public float rotationSpeed = 15f;

    [Header("Player Projectile")]
    public PlayerBullet defaultBulletPrefab;
    public Transform firePoint;
    public float defaultFireHeight = 1f;

    [Header("Optional Ammo UI")]
    public TMP_Text ammoText;

    private class Magazine
    {
        public int remaining;
        public double nextShot;
        public double reloadEnd = -1d;
    }

    private readonly Dictionary<WeaponData, Magazine> magazines =
        new Dictionary<WeaponData, Magazine>();

    private readonly HashSet<EnemyHealth> damagedEnemies =
        new HashSet<EnemyHealth>();

    private Camera mainCamera;
    private PlayerHealth playerHealth;
    private WeaponData weapon;

    private float cooldownTimer;

    private bool isCharging;
    private bool statsInitialized;
    private bool requireRelease;
    private bool warnedMissingBullet;

    private double chargeStartedAt;

    public bool IsRanged => weapon != null && weapon.isRanged;
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
            BlockInput();
            return;
        }

        cooldownTimer = Mathf.Max(
            0f, cooldownTimer - Time.deltaTime);

        if (PointerOverUI())
        {
            BlockInput();
            return;
        }

        if (!mouse.leftButton.isPressed)
            requireRelease = false;

        // 远程武器：按住连续发射。
        if (IsRanged)
        {
            CancelCharge();

            if (mouse.leftButton.isPressed && !requireRelease)
                TryFire();

            return;
        }

        // 近战武器：保留按下蓄力、松开发动攻击。
        if (mouse.leftButton.wasPressedThisFrame &&
            cooldownTimer <= 0f &&
            !requireRelease)
        {
            isCharging = true;
            chargeStartedAt = Time.timeAsDouble;
        }

        if (!isCharging) return;

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

    private Magazine GetMagazine()
    {
        if (!magazines.TryGetValue(weapon, out Magazine magazine))
        {
            magazine = new Magazine
            {
                remaining = Mathf.Max(1, weapon.magazineSize)
            };

            magazines.Add(weapon, magazine);
        }

        if (magazine.reloadEnd >= 0d &&
            Time.timeAsDouble >= magazine.reloadEnd)
        {
            magazine.remaining = Mathf.Max(1, weapon.magazineSize);
            magazine.reloadEnd = -1d;
        }

        return magazine;
    }

    private void TryFire()
    {
        Magazine magazine = GetMagazine();
        double now = Time.timeAsDouble;

        if (magazine.reloadEnd >= 0d ||
            now < magazine.nextShot)
            return;

        PlayerBullet prefab = weapon.bulletPrefab != null
            ? weapon.bulletPrefab
            : defaultBulletPrefab;

        if (prefab == null)
        {
            if (!warnedMissingBullet)
            {
                Debug.LogWarning(
                    "Assign a PlayerBullet prefab on PlayerAttack.",
                    this);
            }

            warnedMissingBullet = true;
            return;
        }

        if (!GetMouseDirection(out Vector3 direction))
            return;

        transform.rotation = Quaternion.LookRotation(direction);

        Vector3 origin = firePoint != null
            ? firePoint.position
            : transform.position + Vector3.up * defaultFireHeight;

        PlayerBullet bullet = Instantiate(
            prefab,
            origin,
            Quaternion.LookRotation(direction));

        bullet.Initialize(
            direction,
            transform,
            weapon,
            enemyLayer,
            projectileObstacleLayers);

        magazine.remaining--;

        magazine.nextShot =
            now + Mathf.Max(0.01f, weapon.shotInterval);

        // 最后一发射出后，开始等待。
        if (magazine.remaining <= 0)
        {
            magazine.reloadEnd =
                now + Mathf.Max(0.01f, weapon.reloadDuration);
        }
    }

    private void LateUpdate()
    {
        if (ammoText == null) return;

        ammoText.gameObject.SetActive(IsRanged);

        if (!IsRanged) return;

        Magazine magazine = GetMagazine();

        ammoText.text = magazine.reloadEnd >= 0d
            ? $"{weapon.weaponName}\n" +
              $"Reload: {magazine.reloadEnd - Time.timeAsDouble:0.0}s"
            : $"{weapon.weaponName}\n" +
              $"{magazine.remaining} / {Mathf.Max(1, weapon.magazineSize)}";
    }

    private void ReleaseAttack(bool charged)
    {
        if (IsRanged ||
            cooldownTimer > 0f ||
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
        BlockInput();

        weapon = data;
        warnedMissingBullet = false;

        RefreshAttackStats();
    }

    public void RemoveWeaponStats()
    {
        ApplyWeaponStats(null);
    }

    public void RefreshAttackStats()
    {
        InitializeStats();

        attackDamage = IsRanged
            ? Mathf.Max(0f, weapon.bulletDamage)
            : baseAttackDamage *
              (weapon == null
                  ? 1f
                  : Mathf.Max(0f, weapon.damageMultiplier));

        attackCooldown = weapon == null
            ? Mathf.Max(0.001f, baseAttackCooldown)
            : weapon.GetAttackInterval(baseAttackCooldown);

        if (weapon != null && weapon.useCustomRange)
        {
            attackRange = Mathf.Max(0f, weapon.customAttackRange);
            attackRadius = Mathf.Max(0.01f, weapon.customAttackRadius);
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

    private void BlockInput()
    {
        CancelCharge();
        requireRelease = true;
    }

    private void OnDisable()
    {
        BlockInput();
    }

    private void OnApplicationFocus(bool focused)
    {
        if (!focused) BlockInput();
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused) BlockInput();
    }

    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying && IsRanged)
            return;

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position + transform.forward *
            (Application.isPlaying ? attackRange : baseAttackRange),
            Application.isPlaying ? attackRadius : baseAttackRadius);
    }
}