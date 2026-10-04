using UnityEngine;

[DisallowMultipleComponent]
public class PlayerBullet : MonoBehaviour
{
    [Min(0.01f)] public float hitRadius = 0.12f;

    private Transform owner;
    private Vector3 direction;

    private float speed;
    private float damage;
    private float life;
    private float poisonDamage;

    private int poisonSeconds;
    private int enemyMask;
    private int obstacleMask;

    private bool initialized;
    private bool finished;

    private void Awake()
    {
        // 子弹使用路径检测，不使用自身 Collider 推动物体。
        foreach (Collider c in GetComponentsInChildren<Collider>(true))
            c.enabled = false;

        Rigidbody body = GetComponent<Rigidbody>();

        if (body != null)
        {
            body.isKinematic = true;
            body.useGravity = false;
        }
    }

    public void Initialize(
        Vector3 aim,
        Transform shooter,
        WeaponData data,
        LayerMask enemies,
        LayerMask obstacles)
    {
        owner = shooter;
        direction = aim.normalized;

        // 复制发射时的数据，切换武器不会改变已发射子弹。
        speed = Mathf.Max(0.01f, data.bulletSpeed);
        damage = Mathf.Max(0f, data.bulletDamage);
        life = Mathf.Max(0.01f, data.bulletLifetime);

        poisonDamage = Mathf.Max(0f, data.poisonDamagePerSecond);
        poisonSeconds = Mathf.Max(0, data.poisonDurationSeconds);

        enemyMask = enemies.value;
        obstacleMask = obstacles.value;

        initialized = true;
        finished = false;
    }

    private void Update()
    {
        if (!initialized || finished || Time.timeScale <= 0f)
            return;

        float step = Mathf.Min(Time.deltaTime, life);

        Advance(speed * step);

        life -= step;

        if (life <= 0f)
            Finish();
    }

    private bool CanHit(Collider c)
    {
        if (c == null || c.transform.IsChildOf(transform))
            return false;

        if (owner != null && c.transform.IsChildOf(owner))
            return false;

        if (c.GetComponentInParent<PlayerHealth>() != null)
            return false;

        if (c.GetComponentInParent<CompanionCombat>() != null)
            return false;

        return IsObstacle(c) || GetEnemy(c) != null;
    }

    private bool IsObstacle(Collider c)
    {
        return !c.isTrigger &&
               (obstacleMask & (1 << c.gameObject.layer)) != 0;
    }

    private EnemyHealth GetEnemy(Collider c)
    {
        if ((enemyMask & (1 << c.gameObject.layer)) == 0)
            return null;

        EnemyHealth enemy = c.GetComponentInParent<EnemyHealth>();

        return enemy != null && !enemy.IsDead()
            ? enemy
            : null;
    }

    private void Advance(float distance)
    {
        int mask = enemyMask | obstacleMask;
        float radius = Mathf.Max(0.01f, hitRadius);

        Collider target = null;

        // 处理子弹出生时已经接触敌人或墙的情况。
        foreach (Collider c in Physics.OverlapSphere(
                     transform.position,
                     radius,
                     mask,
                     QueryTriggerInteraction.Collide))
        {
            if (!CanHit(c)) continue;

            // 同时重叠时，墙优先阻挡。
            if (IsObstacle(c))
            {
                Hit(c);
                return;
            }

            target = c;
        }

        if (target != null)
        {
            Hit(target);
            return;
        }

        if (distance <= 0f) return;

        float nearest = float.PositiveInfinity;

        foreach (RaycastHit hit in Physics.SphereCastAll(
                     transform.position,
                     radius,
                     direction,
                     distance,
                     mask,
                     QueryTriggerInteraction.Collide))
        {
            if (!CanHit(hit.collider)) continue;

            if (hit.distance < nearest ||
                (Mathf.Approximately(hit.distance, nearest) &&
                 IsObstacle(hit.collider)))
            {
                nearest = hit.distance;
                target = hit.collider;
            }
        }

        transform.position += direction *
            (target == null ? distance : nearest);

        if (target != null)
            Hit(target);
    }

    private void Hit(Collider c)
    {
        if (finished) return;

        finished = true;

        EnemyHealth enemy = GetEnemy(c);

        if (enemy != null)
        {
            enemy.TakeDamage(damage);

            if (!enemy.IsDead() &&
                poisonDamage > 0f &&
                poisonSeconds > 0)
            {
                EnemyPoison poison =
                    enemy.GetComponent<EnemyPoison>();

                if (poison == null)
                {
                    poison = enemy.gameObject
                        .AddComponent<EnemyPoison>();
                }

                poison.Apply(poisonDamage, poisonSeconds);
            }
        }

        Destroy(gameObject);
    }

    private void Finish()
    {
        if (finished) return;

        finished = true;
        Destroy(gameObject);
    }
}