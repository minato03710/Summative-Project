using System.Collections.Generic;
using UnityEngine;

public class BossSummonerAI : BossBaseAI
{
    [Header("Movement")]
    [Min(0f)] public float moveSpeed = 1.3f;
    [Min(0f)] public float stoppingDistance = 5f;

    [Header("Summoning")]
    public MeleeEnemyAI minionPrefab;

    [Min(0.1f)] public float summonInterval = 10f;
    [Min(0.1f)] public float castDuration = 3f;
    [Min(1f)] public float summonRadius = 3f;

    [Header("One Ring During Each Cast")]
    public EnemyBullet bulletPrefab;

    [Min(1)] public int ringBulletCount = 16;

    public float ringTime = 1.5f;
    public float bulletHeight = 1f;
    public float bulletSpawnRadius = 0.8f;
    public float bulletDamage = 10f;
    public float bulletSpeed = 8f;

    private bool engaged;
    private bool casting;
    private bool ringFired;

    private double nextCast;
    private double castStarted;

    protected override void TickAI()
    {
        double now = Time.timeAsDouble;
        Vector3 direction = ToPlayer();

        if (!engaged)
        {
            if (direction.magnitude > detectionRange)
            {
                MoveGround(Vector3.zero);
                return;
            }

            engaged = true;
            nextCast = now + Mathf.Max(0.1f, summonInterval);
        }

        if (!casting && now >= nextCast)
        {
            casting = true;
            ringFired = false;
            castStarted = now;

            nextCast = now +
                Mathf.Max(summonInterval, castDuration + 0.1f);
        }

        if (casting)
        {
            // 施法期间只处理重力，不向玩家移动。
            MoveGround(Vector3.zero);

            if (!ringFired &&
                now - castStarted >=
                Mathf.Clamp(ringTime, 0f, castDuration))
            {
                ringFired = true;
                FireRing();
            }

            if (now - castStarted >= castDuration)
            {
                SummonThree();
                casting = false;
            }

            return;
        }

        Face(direction);

        MoveGround(
            direction.magnitude > stoppingDistance
                ? direction.normalized * moveSpeed
                : Vector3.zero);
    }

    private void FireRing()
    {
        if (bulletPrefab == null) return;

        int count = Mathf.Max(1, ringBulletCount);

        for (int i = 0; i < count; i++)
        {
            float angle = i * Mathf.PI * 2f / count;

            Vector3 direction = new Vector3(
                Mathf.Cos(angle),
                0f,
                Mathf.Sin(angle));

            EnemyBullet bullet = Instantiate(
                bulletPrefab,
                transform.position +
                Vector3.up * bulletHeight +
                direction * bulletSpawnRadius,
                Quaternion.LookRotation(direction));

            bullet.speed = bulletSpeed;
            bullet.damage = bulletDamage;
            bullet.obstacleLayer = obstacleLayers;

            bullet.SetDirection(direction);
        }
    }

    private void SummonThree()
    {
        if (minionPrefab == null) return;

        CharacterController body =
            minionPrefab.GetComponent<CharacterController>();

        if (body == null)
        {
            Debug.LogWarning(
                "Minion prefab needs a CharacterController.",
                this);

            return;
        }

        Vector3 scale = minionPrefab.transform.lossyScale;

        float radius = body.radius *
            Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.z));

        float height =
            body.height * Mathf.Abs(scale.y);

        float rootHeight =
            (body.height * 0.5f - body.center.y) *
            Mathf.Abs(scale.y);

        var positions = new List<Vector3>();

        int blockers =
            obstacleLayers.value | (1 << gameObject.layer);

        for (int attempt = 0;
             attempt < 90 && positions.Count < 3;
             attempt++)
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);

            Vector3 offset = new Vector3(
                Mathf.Cos(angle),
                0f,
                Mathf.Sin(angle));

            offset *= Random.Range(
                summonRadius, summonRadius + 2f);

            if (!TryGround(
                    transform.position + offset,
                    radius,
                    height,
                    blockers,
                    out Vector3 point))
                continue;

            bool clear = true;

            foreach (Vector3 previous in positions)
            {
                if (Vector3.Distance(previous, point) <
                    radius * 2f + 0.3f)
                {
                    clear = false;
                }
            }

            if (!clear) continue;

            positions.Add(point);

            MeleeEnemyAI minion = Instantiate(
                minionPrefab,
                point + Vector3.up * rootHeight,
                Quaternion.identity);

            minion.player = player;

            minion.detectionRange = Mathf.Max(
                minion.detectionRange,
                detectionRange + summonRadius + 2f);
        }

        if (positions.Count < 3)
        {
            Debug.LogWarning(
                "Not enough clear ground to summon all three minions.",
                this);
        }
    }

    protected override void ResetAttack()
    {
        casting = false;
        ringFired = false;

        nextCast =
            Time.timeAsDouble +
            Mathf.Max(0.1f, summonInterval);
    }
}