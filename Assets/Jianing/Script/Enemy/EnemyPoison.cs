using UnityEngine;
using UnityEngine.VFX;

[DisallowMultipleComponent]
[RequireComponent(typeof(EnemyHealth))]
public class EnemyPoison : MonoBehaviour
{
    public VisualEffect PoisonVFX;

    private EnemyHealth health;
    private float damagePerTick;

    private double nextTick;
    private double expiresAt;
    private bool active;

    private void Awake()
    {
        health = GetComponent<EnemyHealth>();
        SimpleVFX.Prepare(PoisonVFX);
    }

    public void Apply(float damagePerSecond, int durationSeconds)
    {
        if (health == null)
            health = GetComponent<EnemyHealth>();

        if (health == null ||
            health.IsDead() ||
            health.IsReviving ||
            damagePerSecond <= 0f ||
            durationSeconds <= 0)
            return;

        Tick();

        if (health.IsDead() || health.IsReviving)
            return;

        double now = Time.timeAsDouble;

        // 再次中毒时保留下一次扣血时间。
        if (!active)
            nextTick = now + 1d;

        damagePerTick = damagePerSecond;
        expiresAt = now + durationSeconds;

        active = true;
        enabled = true;

        SimpleVFX.Loop(PoisonVFX, true);
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

        while (nextTick <= now + 0.000001d &&
               nextTick <= expiresAt + 0.000001d)
        {
            nextTick += 1d;
            health.TakeDamage(damagePerTick);

            if (!active || health.IsDead() || health.IsReviving)
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
        SimpleVFX.Prepare(PoisonVFX);
    }
}