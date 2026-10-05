using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(EnemyHealth))]
public abstract class BossBaseAI : MonoBehaviour
{
    [Header("Target and Movement")]
    public Transform player;
    [Min(0f)] public float detectionRange = 25f;
    public float gravity = -25f;

    [Header("Layers")]
    public LayerMask groundLayers;
    public LayerMask obstacleLayers;
    public LayerMask playerLayers = ~0;

    [Header("Second Life Fire")]
    public FireTile fireTilePrefab;
    [Min(0)] public int fireCount = 12;

    public float fireMinRadius = 2.5f;
    public float fireMaxRadius = 7f;

    [Min(0.1f)] public float fireGridSize = 1.5f;
    [Min(0f)] public float fireLifetime = 30f;

    protected CharacterController controller;
    protected EnemyHealth health;
    protected PlayerHealth targetHealth;
    protected float verticalSpeed;

    private readonly HashSet<PlayerHealth> frameHits =
        new HashSet<PlayerHealth>();

    protected virtual void Awake()
    {
        controller = GetComponent<CharacterController>();
        health = GetComponent<EnemyHealth>();

        health.useSecondLife = true;
        health.SecondLifeStarted += BeginSecondLife;
    }

    protected virtual void Update()
    {
        if (Time.timeScale <= 0f ||
            health.IsDead() ||
            !controller.enabled)
            return;

        if (player == null)
        {
            GameObject found =
                GameObject.FindGameObjectWithTag("Player");

            if (found != null)
                player = found.transform;
        }

        targetHealth = player != null
            ? player.GetComponentInParent<PlayerHealth>()
            : null;

        if (player == null ||
            targetHealth == null ||
            targetHealth.IsDead() ||
            health.IsReviving)
        {
            MoveGround(Vector3.zero);
            return;
        }

        TickAI();
    }

    protected abstract void TickAI();
    protected abstract void ResetAttack();

    protected Vector3 ToPlayer()
    {
        Vector3 delta = player.position - transform.position;
        delta.y = 0f;
        return delta;
    }

    protected void Face(Vector3 direction)
    {
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(direction);
    }

    protected CollisionFlags MoveGround(Vector3 velocity)
    {
        if (controller.isGrounded && verticalSpeed < 0f)
            verticalSpeed = -2f;

        verticalSpeed += gravity * Time.deltaTime;

        CollisionFlags flags = controller.Move(
            (velocity + Vector3.up * verticalSpeed) *
            Time.deltaTime);

        if ((flags & CollisionFlags.Below) != 0)
            verticalSpeed = -2f;

        return flags;
    }

    protected float ClearDistance(
        Vector3 direction,
        float distance)
    {
        Vector3 scale = transform.lossyScale;

        float radius = controller.radius *
            Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.z));

        float half = Mathf.Max(
            0f,
            controller.height * Mathf.Abs(scale.y) * 0.5f -
            radius);

        Vector3 center =
            transform.TransformPoint(controller.center);

        float skin = Mathf.Max(0.02f, controller.skinWidth);

        if (Physics.CapsuleCast(
                center + Vector3.up * half,
                center - Vector3.up * half,
                radius,
                direction,
                out RaycastHit hit,
                distance + skin,
                obstacleLayers,
                QueryTriggerInteraction.Ignore))
        {
            return Mathf.Max(0f, hit.distance - skin);
        }

        return distance;
    }

    protected bool TryGround(
        Vector3 candidate,
        float radius,
        float height,
        LayerMask blockers,
        out Vector3 point)
    {
        point = candidate;

        if (!Physics.Raycast(
                candidate + Vector3.up * 8f,
                Vector3.down,
                out RaycastHit hit,
                24f,
                groundLayers,
                QueryTriggerInteraction.Ignore))
            return false;

        // 避免把召唤物放到过陡的表面。
        if (Vector3.Dot(hit.normal, Vector3.up) < 0.8f)
            return false;

        point = hit.point + Vector3.up * 0.05f;

        return !Physics.CheckCapsule(
            point + Vector3.up * radius,
            point + Vector3.up * Mathf.Max(radius, height - radius),
            radius,
            blockers,
            QueryTriggerInteraction.Ignore);
    }

    protected void DamageArea(
        Vector3 center,
        float radius,
        float damage,
        HashSet<PlayerHealth> hitOnce = null,
        float forwardDot = -1f)
    {
        frameHits.Clear();

        foreach (Collider hit in Physics.OverlapSphere(
                     center,
                     radius,
                     playerLayers,
                     QueryTriggerInteraction.Ignore))
        {
            DamageCollider(
                hit, center, damage, hitOnce, forwardDot);
        }
    }

    protected void DamagePath(
        Vector3 from,
        Vector3 to,
        float radius,
        float damage,
        HashSet<PlayerHealth> hitOnce)
    {
        DamageArea(from, radius, damage, hitOnce);

        Vector3 delta = to - from;

        if (delta.magnitude > 0.001f)
        {
            foreach (RaycastHit hit in Physics.SphereCastAll(
                         from,
                         radius,
                         delta.normalized,
                         delta.magnitude,
                         playerLayers,
                         QueryTriggerInteraction.Ignore))
            {
                DamageCollider(
                    hit.collider, from, damage, hitOnce, -1f);
            }
        }

        DamageArea(to, radius, damage, hitOnce);
    }

    private void DamageCollider(
        Collider hit,
        Vector3 origin,
        float damage,
        HashSet<PlayerHealth> hitOnce,
        float forwardDot)
    {
        PlayerHealth victim =
            hit.GetComponentInParent<PlayerHealth>();

        if (victim == null ||
            victim.IsDead() ||
            frameHits.Contains(victim) ||
            (hitOnce != null && hitOnce.Contains(victim)))
            return;

        Vector3 delta =
            victim.transform.position - transform.position;

        delta.y = 0f;

        if (forwardDot > -1f &&
            delta.sqrMagnitude > 0.001f &&
            Vector3.Dot(transform.forward, delta.normalized) < forwardDot)
            return;

        // 墙壁可以阻挡伤害。
        if (Physics.Linecast(
                origin,
                hit.ClosestPoint(origin),
                obstacleLayers,
                QueryTriggerInteraction.Ignore))
            return;

        frameHits.Add(victim);
        hitOnce?.Add(victim);

        victim.TakeDamage(damage);
    }

    private void BeginSecondLife()
    {
        ResetAttack();
        SpawnFire();
    }

    private void SpawnFire()
    {
        if (fireTilePrefab == null || fireCount <= 0)
            return;

        float grid = Mathf.Max(0.1f, fireGridSize);
        float outer = Mathf.Max(grid, fireMaxRadius);
        float inner = Mathf.Clamp(fireMinRadius, 0f, outer);

        int extent = Mathf.CeilToInt(outer / grid);

        var used = new HashSet<Vector2Int>();
        int spawned = 0;

        for (int attempt = 0;
             attempt < fireCount * 40 && spawned < fireCount;
             attempt++)
        {
            Vector2Int cell = new Vector2Int(
                Random.Range(-extent, extent + 1),
                Random.Range(-extent, extent + 1));

            Vector3 offset = new Vector3(
                cell.x * grid, 0f, cell.y * grid);

            if (offset.magnitude < inner ||
                offset.magnitude > outer ||
                !used.Add(cell))
                continue;

            if (!TryGround(
                    transform.position + offset,
                    grid * 0.45f,
                    1f,
                    obstacleLayers,
                    out Vector3 point))
                continue;

            FireTile tile = Instantiate(
                fireTilePrefab,
                point,
                Quaternion.identity);

            tile.SetLifetime(fireLifetime);
            spawned++;
        }

        if (spawned < fireCount)
        {
            Debug.LogWarning(
                $"Only found space for {spawned}/{fireCount} fire tiles.",
                this);
        }
    }

    protected virtual void OnDestroy()
    {
        if (health != null)
            health.SecondLifeStarted -= BeginSecondLife;
    }
}