using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class FireTile : MonoBehaviour
{
    [Min(0f)] public float damagePerTick = 5f;
    [Min(0.05f)] public float damageInterval = 1f;

    [Min(0f)] public float activationDelay = 0.75f;

    [Tooltip("0 = permanent")]
    [Min(0f)] public float lifetime = 0f;

    public LayerMask playerLayers = ~0;

    private BoxCollider area;
    private double activatesAt;
    private double expiresAt;

    private readonly HashSet<PlayerHealth> checkedPlayers =
        new HashSet<PlayerHealth>();

    private readonly Dictionary<PlayerHealth, double> nextDamage =
        new Dictionary<PlayerHealth, double>();

    private void Awake()
    {
        area = GetComponent<BoxCollider>();
        area.isTrigger = true;
    }

    private void OnEnable()
    {
        nextDamage.Clear();

        activatesAt =
            Time.timeAsDouble +
            Mathf.Max(0f, activationDelay);

        SetLifetime(lifetime);
    }

    public void SetLifetime(float seconds)
    {
        lifetime = Mathf.Max(0f, seconds);

        expiresAt = lifetime > 0f
            ? Time.timeAsDouble + lifetime
            : double.PositiveInfinity;
    }

    private void Update()
    {
        if (Time.timeScale <= 0f)
            return;

        double now = Time.timeAsDouble;

        if (now >= expiresAt)
        {
            Destroy(gameObject);
            return;
        }

        if (now < activatesAt || area == null)
            return;

        Vector3 scale = transform.lossyScale;

        scale = new Vector3(
            Mathf.Abs(scale.x),
            Mathf.Abs(scale.y),
            Mathf.Abs(scale.z));

        Vector3 halfSize =
            Vector3.Scale(area.size, scale) * 0.5f;

        Vector3 center =
            transform.TransformPoint(area.center);

        checkedPlayers.Clear();

        foreach (Collider hit in Physics.OverlapBox(
                     center,
                     halfSize,
                     transform.rotation,
                     playerLayers,
                     QueryTriggerInteraction.Ignore))
        {
            PlayerHealth player =
                hit.GetComponentInParent<PlayerHealth>();

            if (player == null ||
                player.IsDead() ||
                !checkedPlayers.Add(player))
                continue;

            if (nextDamage.TryGetValue(player, out double next) &&
                now < next)
                continue;

            nextDamage[player] =
                now + Mathf.Max(0.05f, damageInterval);

            player.TakeDamage(damagePerTick);
        }
    }
}