using UnityEngine;

public class ResourcePickup : MonoBehaviour
{
    public enum ResourceType
    {
        Wood,
        Stone,
        Food,
        Gold
    }


[Header("Resource")]
    public ResourceType resourceType = ResourceType.Wood;
    public int amount = 1;

    [Header("Auto Pickup")]
    public float pickupRange = 3f;
    public float attractionSpeed = 12f;
    public float collectDistance = 0.5f;

    private Transform player;
    private bool isAttracted = false;
    private bool collected = false;

    void Update()
    {
        if (collected)
            return;

        // 如果还没有找到玩家
        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }

            return;
        }

        // 计算玩家距离
        float distance =
            Vector3.Distance(
                transform.position,
                player.position
            );

        // 玩家进入吸附范围
        if (distance <= pickupRange)
        {
            isAttracted = true;
        }

        // 开始飞向玩家
        if (isAttracted)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                player.position,
                attractionSpeed * Time.deltaTime
            );

            // 到达玩家
            if (Vector3.Distance(
                transform.position,
                player.position
            ) <= collectDistance)
            {
                Collect();
            }
        }
    }

    void Collect()
    {
        if (collected)
            return;

        collected = true;

        ResourceManager resourceManager =
            player.GetComponent<ResourceManager>();

        if (resourceManager != null)
        {
            resourceManager.AddResource(
                resourceType,
                amount
            );

            Debug.Log(
                "Collected " +
                resourceType +
                " x" +
                amount
            );
        }

        Destroy(gameObject);
    }


}


