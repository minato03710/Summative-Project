using UnityEngine;


[RequireComponent(typeof(EnemyHealth))]
public class PowerCell : MonoBehaviour
{
    [Header("Fracture Settings")]
    [SerializeField] private GameObject fracturedPrefab;

    [SerializeField]
    private float explosionForce = 300f;

    [SerializeField, Min(0.1f)]
    private float explosionRadius = 3f;

    [SerializeField, Min(0.1f)]
    private float fragmentsLifetime = 4f;

    private EnemyHealth enemyHealth;
    private PowerCellManager registeredManager;

    private bool registered;
    private bool deathHandled;

    public bool IsDestroyed =>
        deathHandled ||
        (enemyHealth != null && enemyHealth.IsDead());

    private void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();

        // PowerCell 只有一条命。
        enemyHealth.useSecondLife = false;

        // 监听 EnemyHealth 的最终死亡事件。
        enemyHealth.Died += HandleDeath;
    }

    private void Start()
    {
        RegisterWithManager();
    }

    private void RegisterWithManager()
    {
        if (registered || deathHandled)
            return;

        PowerCellManager manager = PowerCellManager.Instance;

        if (manager == null)
            return;

        registeredManager = manager;
        registered = true;

        registeredManager.RegisterCell();
    }

    // 兼容原来的整数伤害调用。
    public void TakeDamage(int damage = 1)
    {
        TakeDamage((float)damage);
    }

    // 所有伤害统一交给 EnemyHealth。
    public void TakeDamage(float damage)
    {
        if (IsDestroyed || damage <= 0f)
            return;

        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
        }
    }

    public float GetCurrentHealth()
    {
        return enemyHealth != null
            ? enemyHealth.GetCurrentHealth()
            : 0f;
    }

    public void Break()
    {
        if (IsDestroyed || enemyHealth == null)
            return;

        // 通过同一套死亡流程破坏电池。
        enemyHealth.TakeDamage(
            enemyHealth.GetCurrentHealth()
        );
    }

    private void HandleDeath()
    {
        if (deathHandled)
            return;

        RegisterWithManager();
        deathHandled = true;

        // 立即关闭碰撞，避免本帧内继续挡住其他子弹。
        foreach (Collider cellCollider
                 in GetComponentsInChildren<Collider>(true))
        {
            cellCollider.enabled = false;
        }

        SpawnFragments();

        if (registered && registeredManager != null)
        {
            registeredManager.OnCellDestroyed();
        }

        // 原物体由 EnemyHealth 统一销毁。
    }

    private void SpawnFragments()
    {
        if (fracturedPrefab == null)
            return;

        GameObject fragments = Instantiate(
            fracturedPrefab,
            transform.position,
            transform.rotation
        );

        foreach (Rigidbody rb
                 in fragments.GetComponentsInChildren<Rigidbody>())
        {
            rb.AddExplosionForce(
                explosionForce,
                transform.position,
                explosionRadius
            );
        }

        Destroy(fragments, fragmentsLifetime);
    }

    private void OnDestroy()
    {
        if (enemyHealth != null)
        {
            enemyHealth.Died -= HandleDeath;
        }
    }

    [ContextMenu("Test Break Cell")]
    public void TestBreakCell()
    {
        if (Application.isPlaying)
        {
            Break();
        }
    }
}