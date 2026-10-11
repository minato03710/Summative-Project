using UnityEngine;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    public PlayerHealth player;
    public EnemyHealth[] enemyPrefabs;
    public Transform[] spawnPoints;

    [Header("Population")]
    [Min(0)] public int targetEnemyCount = 10;
    [Min(0.1f)] public float respawnCooldown = 30f;
    public bool excludePowerCells = true;

    [Header("Spawn Position")]
    public LayerMask groundLayers;
    public LayerMask blockingLayers = ~0;

    [Min(0f)] public float minPlayerDistance = 3f;
    [Min(0.1f)] public float groundCheckHeight = 2f;
    [Min(0.1f)] public float groundCheckDepth = 5f;

    [Header("Runtime Status")]
    [SerializeField] private int currentEnemyCount;
    [SerializeField] private bool coolingDown;
    [SerializeField] private float cooldownRemaining;
    [SerializeField] private int totalSpawned;

    private readonly List<EnemyHealth> validPrefabs =
        new List<EnemyHealth>();

    private const float CountCheckInterval = 0.25f;
    private const float BlockedRetryDelay = 1f;

    private float countCheckTimer;
    private double nextSpawnTime;
    private bool warnedBlocked;

    private void Start()
    {
        if (player == null)
        {
            player = FindFirstObjectByType<PlayerHealth>();
        }

        if (enemyPrefabs != null)
        {
            foreach (EnemyHealth prefab in enemyPrefabs)
            {
                if (prefab == null)
                    continue;

                CharacterController controller =
                    prefab.GetComponent<CharacterController>();

                bool isAllowed =
                    !excludePowerCells ||
                    prefab.GetComponentInParent<PowerCell>() == null;

                if (controller != null &&
                    controller.enabled &&
                    isAllowed)
                {
                    validPrefabs.Add(prefab);
                }
            }
        }

        if (player == null ||
            validPrefabs.Count == 0 ||
            groundLayers.value == 0)
        {
            Debug.LogError(
                "EnemySpawner: Assign Player, enemy prefabs with " +
                "EnemyHealth + CharacterController, and Ground Layers.",
                this
            );

            enabled = false;
            return;
        }

        RefreshEnemyCount();
    }

    private void Update()
    {
        if (Time.timeScale <= 0f)
            return;

        // 定期统计整个场景中的存活敌人。
        countCheckTimer -= Time.deltaTime;

        if (countCheckTimer <= 0f)
        {
            RefreshEnemyCount();
            countCheckTimer = CountCheckInterval;
        }

        if (player == null ||
            player.IsDead() ||
            currentEnemyCount >= Mathf.Max(0, targetEnemyCount))
        {
            CancelCooldown();
            return;
        }

        // 数量不足时开始冷却。
        // 冷却期间继续有敌人死亡，不会重新计时。
        if (!coolingDown)
        {
            BeginCooldown(respawnCooldown);
        }

        cooldownRemaining = Mathf.Max(
            0f,
            (float)(nextSpawnTime - Time.timeAsDouble)
        );

        if (cooldownRemaining > 0f)
            return;

        // 生成前再次统计，防止其他来源已经把敌人数补满。
        RefreshEnemyCount();

        if (currentEnemyCount >= Mathf.Max(0, targetEnemyCount))
        {
            CancelCooldown();
            return;
        }

        bool spawned = TrySpawnOne();

        RefreshEnemyCount();

        if (currentEnemyCount >= Mathf.Max(0, targetEnemyCount))
        {
            CancelCooldown();
        }
        else
        {
            // 成功生成一只后，再等待完整的冷却时间。
            // 位置被挡住时，每秒重试一次寻找空位。
            BeginCooldown(
                spawned ? respawnCooldown : BlockedRetryDelay
            );
        }
    }

    private void RefreshEnemyCount()
    {
        currentEnemyCount = 0;

        EnemyHealth[] enemies = FindObjectsByType<EnemyHealth>(
            FindObjectsInactive.Exclude,
            FindObjectsSortMode.None
        );

        foreach (EnemyHealth enemy in enemies)
        {
            if (enemy.IsDead())
                continue;

            if (excludePowerCells &&
                enemy.GetComponentInParent<PowerCell>() != null)
            {
                continue;
            }

            currentEnemyCount++;
        }
    }

    private void BeginCooldown(float duration)
    {
        coolingDown = true;
        cooldownRemaining = Mathf.Max(0.1f, duration);

        nextSpawnTime =
            Time.timeAsDouble + cooldownRemaining;
    }

    private void CancelCooldown()
    {
        coolingDown = false;
        cooldownRemaining = 0f;
    }

    private bool TrySpawnOne()
    {
        EnemyHealth prefab = validPrefabs[
            Random.Range(0, validPrefabs.Count)
        ];

        if (prefab == null)
            return false;

        Physics.SyncTransforms();

        if (!TryGetSpawnPose(
                prefab,
                out Vector3 position,
                out Quaternion rotation))
        {
            if (!warnedBlocked)
            {
                Debug.LogWarning(
                    "EnemySpawner: No free spawn point. Check ground, " +
                    "obstacles and player distance. Retrying in one second.",
                    this
                );

                warnedBlocked = true;
            }

            return false;
        }

        EnemyHealth enemy = Instantiate(
            prefab,
            position,
            rotation
        );

        // 接入现有的普通敌人 AI。
        EnemyAI normalAI = enemy.GetComponent<EnemyAI>();

        if (normalAI != null)
        {
            normalAI.player = player.transform;
        }

        // 也兼容之前制作的 Boss AI。
        BossBaseAI bossAI = enemy.GetComponent<BossBaseAI>();

        if (bossAI != null)
        {
            bossAI.player = player.transform;
        }

        enemy.gameObject.SetActive(true);

        totalSpawned++;
        warnedBlocked = false;

        return true;
    }

    private bool TryGetSpawnPose(
        EnemyHealth prefab,
        out Vector3 position,
        out Quaternion rotation)
    {
        position = Vector3.zero;
        rotation = Quaternion.identity;

        CharacterController controller =
            prefab.GetComponent<CharacterController>();

        if (controller == null)
            return false;

        // 根据敌人实际大小检查生成位置。
        Vector3 scale = prefab.transform.localScale;

        float radius = Mathf.Max(
            0.05f,
            controller.radius *
            Mathf.Max(
                Mathf.Abs(scale.x),
                Mathf.Abs(scale.z)
            )
        );

        float height = Mathf.Max(
            controller.height * Mathf.Abs(scale.y),
            radius * 2f
        );

        bool usePoints =
            spawnPoints != null && spawnPoints.Length > 0;

        int count = usePoints ? spawnPoints.Length : 1;
        int first = Random.Range(0, count);

        // 从随机生成点开始，依次寻找可用的位置。
        for (int i = 0; i < count; i++)
        {
            Transform point = usePoints
                ? spawnPoints[(first + i) % count]
                : transform;

            if (point == null)
                continue;

            Vector3 origin =
                point.position + Vector3.up * groundCheckHeight;

            if (!Physics.Raycast(
                    origin,
                    Vector3.down,
                    out RaycastHit hit,
                    groundCheckHeight + groundCheckDepth,
                    groundLayers,
                    QueryTriggerInteraction.Ignore))
            {
                continue;
            }

            // 跳过过于倾斜的地面。
            if (hit.normal.y < 0.9f)
                continue;

            Vector3 fromPlayer =
                hit.point - player.transform.position;

            fromPlayer.y = 0f;

            if (fromPlayer.sqrMagnitude <
                minPlayerDistance * minPlayerDistance)
            {
                continue;
            }

            Vector3 feet =
                hit.point + Vector3.up * 0.1f;

            Vector3 bottom =
                feet + Vector3.up * radius;

            Vector3 top =
                feet + Vector3.up * (height - radius);

            // 避免生成在墙壁、玩家或其他敌人的身体里面。
            if (Physics.CheckCapsule(
                    bottom,
                    top,
                    radius,
                    blockingLayers,
                    QueryTriggerInteraction.Ignore))
            {
                continue;
            }

            rotation = Quaternion.Euler(
                0f,
                point.eulerAngles.y,
                0f
            );

            // 兼容根坐标在脚底或身体中心的敌人。
            Vector3 centerOffset =
                rotation *
                Vector3.Scale(controller.center, scale);

            position =
                feet +
                Vector3.up * (height * 0.5f) -
                centerOffset;

            return true;
        }

        return false;
    }

    private void OnDisable()
    {
        CancelCooldown();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Gizmos.DrawWireSphere(
                transform.position,
                0.5f
            );

            return;
        }

        foreach (Transform point in spawnPoints)
        {
            if (point != null)
            {
                Gizmos.DrawWireSphere(
                    point.position,
                    0.5f
                );
            }
        }
    }
}