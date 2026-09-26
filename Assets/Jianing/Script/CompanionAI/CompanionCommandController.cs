using UnityEngine;

[DefaultExecutionOrder(-100)]
[DisallowMultipleComponent]
[RequireComponent(typeof(CompanionMovement), typeof(CompanionCombat))]
public class CompanionCommandController : MonoBehaviour
{
    [Header("Player")]
    public PlayerHealth playerHealth;

    [Header("Heal")]
    [Min(0f)]
    public float healAmount = 30f;

    [Min(0f)]
    public float healCooldown = 0f;

    [Header("Boost")]
    [Min(0.1f)]
    public float boostDuration = 5f;

    [Min(1f)]
    public float boostDamageMultiplier = 1.5f;

    [Min(1f)]
    public float boostSpeedMultiplier = 1.5f;

    [Min(0f)]
    public float boostCooldown = 0f;

    [Header("Optional Sitting Animation")]
    public Animator companionAnimator;
    public string sittingParameter = "IsSitting";

    private CompanionMovement movement;
    private CompanionCombat combat;

    private float boostRemaining;
    private float healCooldownRemaining;
    private float boostCooldownRemaining;

    public bool IsSitting { get; private set; }

    public bool IsBoosted => boostRemaining > 0f;

    public float BoostRemaining => boostRemaining;

    public float HealCooldownRemaining => healCooldownRemaining;

    public float BoostCooldownRemaining => boostCooldownRemaining;

    public float DamageMultiplier =>
        IsBoosted ? Mathf.Max(1f, boostDamageMultiplier) : 1f;

    public float SpeedMultiplier =>
        IsBoosted ? Mathf.Max(1f, boostSpeedMultiplier) : 1f;

    public string LastResult { get; private set; } = "";

    public bool CanHeal
    {
        get
        {
            FindPlayer();

            return healCooldownRemaining <= 0f &&
                   healAmount > 0f &&
                   playerHealth != null &&
                   !playerHealth.IsDead() &&
                   playerHealth.GetCurrentHealth() <
                   playerHealth.GetMaxHealth();
        }
    }

    public bool CanBoost => boostCooldownRemaining <= 0f;

    private void Awake()
    {
        movement = GetComponent<CompanionMovement>();
        combat = GetComponent<CompanionCombat>();

        if (companionAnimator == null)
        {
            companionAnimator = GetComponentInChildren<Animator>();
        }

        // 使用模块化同伴脚本，避免旧 AI 重复控制移动和攻击。
        CompanionAI legacyAI = GetComponent<CompanionAI>();

        if (legacyAI != null && legacyAI.enabled)
        {
            legacyAI.enabled = false;

            Debug.LogWarning(
                "Disabled legacy CompanionAI. " +
                "Use CompanionFollow, CompanionCombat and CompanionMovement.",
                this
            );
        }

        FindPlayer();
    }

    private void Update()
    {
        if (Time.timeScale <= 0f)
            return;

        // 按游戏时间计时，暂停期间不会消耗持续时间或冷却。
        boostRemaining =
            Mathf.Max(0f, boostRemaining - Time.deltaTime);

        healCooldownRemaining =
            Mathf.Max(0f, healCooldownRemaining - Time.deltaTime);

        boostCooldownRemaining =
            Mathf.Max(0f, boostCooldownRemaining - Time.deltaTime);
    }

    private void FindPlayer()
    {
        if (playerHealth != null)
            return;

        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            playerHealth = playerObject.GetComponent<PlayerHealth>();
        }
    }

    public void HealPlayer()
    {
        if (!CanHeal)
        {
            LastResult =
                "Healing unavailable: full HP, cooldown, or no living player.";

            return;
        }

        float previousHealth = playerHealth.GetCurrentHealth();

        playerHealth.Heal(Mathf.Max(0f, healAmount));

        healCooldownRemaining = Mathf.Max(0f, healCooldown);

        float restoredHealth =
            playerHealth.GetCurrentHealth() - previousHealth;

        LastResult =
            "Recovered " + restoredHealth.ToString("0.#") + " HP";
    }

    public void BoostCompanion()
    {
        if (!CanBoost)
        {
            LastResult = "Boost is on cooldown.";
            return;
        }

        // 重复释放刷新时间，不叠加倍率。
        boostRemaining = Mathf.Max(0.1f, boostDuration);
        boostCooldownRemaining = Mathf.Max(0f, boostCooldown);

        LastResult =
            "Boost ready for " +
            boostRemaining.ToString("0.#") +
            " seconds of gameplay";
    }

    public void Sit()
    {
        IsSitting = true;

        if (movement != null)
        {
            movement.Stop();
        }

        if (combat != null)
        {
            combat.ClearTarget();
        }

        SetSittingAnimation(true);

        LastResult =
            "Staying here. Follow, attack, jump and teleport are stopped.";
    }

    public void FollowAndAttack()
    {
        IsSitting = false;

        if (combat != null)
        {
            combat.ClearTarget();
        }

        SetSittingAnimation(false);

        LastResult =
            "Following player and attacking the nearest enemy in range.";
    }

    private void SetSittingAnimation(bool sitting)
    {
        if (companionAnimator == null ||
            string.IsNullOrWhiteSpace(sittingParameter))
        {
            return;
        }

        foreach (
            AnimatorControllerParameter parameter
            in companionAnimator.parameters
        )
        {
            if (parameter.name == sittingParameter &&
                parameter.type == AnimatorControllerParameterType.Bool)
            {
                companionAnimator.SetBool(sittingParameter, sitting);
                return;
            }
        }
    }
}
