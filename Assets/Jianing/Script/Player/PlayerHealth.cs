using UnityEngine;

[DisallowMultipleComponent]
public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [Min(1f)] public float maxHealth = 100f;
    public float currentHealth;

    [Header("Respawn")]
    [SerializeField] private string respawnTag = "RespawnPoint";
    [SerializeField] private string fallbackSpawnTag = "SpawnPoint";

    private bool isDead;
    private bool initialized;

    private CharacterController controller;

    private Behaviour[] controls;
    private bool[] controlStates;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        controls = new Behaviour[]
        {
            GetComponent<PlayerMovement>(),
            GetComponent<PlayerDash>(),
            GetComponent<PlayerAttack>()
        };

        controlStates = new bool[controls.Length];
    }

    private void Start()
    {
        InitializeHealth();
    }

    private void InitializeHealth()
    {
        if (initialized)
            return;

        initialized = true;

        maxHealth = Mathf.Max(1f, maxHealth);
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        InitializeHealth();

        if (isDead || damage <= 0f)
            return;

        currentHealth = Mathf.Max(
            0f,
            currentHealth - damage
        );

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        InitializeHealth();

        if (isDead || amount <= 0f)
            return;

        currentHealth = Mathf.Min(
            GetMaxHealth(),
            currentHealth + amount
        );
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;
        currentHealth = 0f;

        for (int i = 0; i < controls.Length; i++)
        {
            if (controls[i] == null)
                continue;

            controlStates[i] = controls[i].enabled;
            controls[i].enabled = false;
        }

        Respawn();
    }

    public void Respawn()
    {
        if (!isDead)
            return;

        Transform target = FindClosestPoint(respawnTag);

        if (target == null)
        {
            target = FindClosestPoint(fallbackSpawnTag);
        }

        if (target == null)
        {
            Debug.LogError(
                "PlayerHealth: No active RespawnPoint or SpawnPoint found. " +
                "Create a safe respawn point and assign the correct Tag.",
                this
            );

            return;
        }

        bool controllerWasEnabled =
            controller != null && controller.enabled;

        if (controller != null)
        {
            controller.enabled = false;
        }

        transform.position = target.position;
        transform.rotation = Quaternion.identity;

        if (controller != null)
        {
            controller.enabled = controllerWasEnabled;
        }

        Physics.SyncTransforms();

   
        currentHealth = GetMaxHealth();
        isDead = false;

        for (int i = 0; i < controls.Length; i++)
        {
            if (controls[i] != null)
            {
                controls[i].enabled = controlStates[i];
            }
        }
    }

    private Transform FindClosestPoint(string pointTag)
    {
        if (string.IsNullOrWhiteSpace(pointTag))
            return null;

        GameObject[] points;

        try
        {
            points = GameObject.FindGameObjectsWithTag(pointTag);
        }
        catch (UnityException)
        {
            Debug.LogError(
                "PlayerHealth: Tag is not defined: " + pointTag,
                this
            );

            return null;
        }

        Transform closest = null;
        float closestDistance = float.PositiveInfinity;

        foreach (GameObject point in points)
        {
            float distance = (
                point.transform.position - transform.position
            ).sqrMagnitude;

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = point.transform;
            }
        }

        return closest;
    }

    public float GetCurrentHealth()
    {
        InitializeHealth();
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return Mathf.Max(1f, maxHealth);
    }

    public bool IsDead()
    {
        return isDead;
    }
}