using UnityEngine;

public class ResourcePickup : MonoBehaviour
{
    // 保持旧枚举的数值，已有拾取物会对应到新资源。
    public enum ResourceType
    {
        Energy = 0,
        Battery = 1,
        Food = 2,
        Scrap = 3
    }

    [Header("Resource")]
    public ResourceType resourceType = ResourceType.Energy;
    [Min(1)] public int amount = 1;

    [Header("Auto Pickup")]
    public float pickupRange = 3f;
    public float attractionSpeed = 12f;
    public float collectDistance = 0.5f;

    private Transform player;
    private bool isAttracted;
    private bool collected;

    private void Update()
    {
        if (collected) return;

        if (player == null)
        {
            GameObject target = GameObject.FindGameObjectWithTag("Player");
            if (target != null) player = target.transform;
            return;
        }

        if (Vector3.Distance(transform.position, player.position) <= pickupRange)
            isAttracted = true;

        if (!isAttracted) return;

        transform.position = Vector3.MoveTowards(
            transform.position, player.position, attractionSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, player.position) <= collectDistance)
            Collect();
    }

    private void Collect()
    {
        if (collected || player == null) return;

        ResourceManager resources = player.GetComponent<ResourceManager>();
        if (resources == null) return;

        collected = true;
        resources.AddResource(resourceType, amount);
        Destroy(gameObject);
    }
}