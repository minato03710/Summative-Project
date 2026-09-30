using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(EnemyHealth))]
public class EnemyPoison : MonoBehaviour
{
    private EnemyHealth health;
    private float damagePerTick;

    private double nextTick;
    private double expiresAt;
    private bool active;

    public void Apply(float damagePerSecond, int durationSeconds)
    {
        if (health == null)
            health = GetComponent<EnemyHealth>();

        if (health == null ||
            health.IsDead() ||
            damagePerSecond <= 0f ||
            durationSeconds <= 0)
            return;

        Tick();

        if (health.IsDead()) return;

        double now = Time.timeAsDouble;

        // 已经中毒时保留下一次扣血时刻。
        // 高频命中不会不断推迟伤害。
        if (!active)
            nextTick = now + 1d;

        damagePerTick = damagePerSecond;
        expiresAt = now + durationSeconds;

        active = true;
        enabled = true;
    }

    private void Update()
    {
        Tick();
    }

    private void Tick()
    {
        if (!active || Time.timeScale <= 0f)
            return;

        if (health == null || health.IsDead())
        {
            Clear();
            return;
        }

        double now = Time.timeAsDouble;

        // 使用游戏时间，暂停期间不会继续计时。
        while (nextTick <= now + 0.000001d &&
               nextTick <= expiresAt + 0.000001d)
        {
            nextTick += 1d;
            health.TakeDamage(damagePerTick);

            if (health.IsDead())
            {
                Clear();
                return;
            }
        }

        if (now >= expiresAt)
            Clear();
    }

    private void Clear()
    {
        active = false;
        enabled = false;
    }

    private void OnDisable()
    {
        active = false;
    }
}