using UnityEngine;

public class EnemyDropSystem : MonoBehaviour
{
    [System.Serializable]
    public class DropItem
    {
        public GameObject prefab;

        [Range(0f, 100f)]
        public float dropChance = 25f;

        public int minAmount = 1;
        public int maxAmount = 1;
    }

    [Header("Drop Table")]
    public DropItem[] dropItems;

    public void DropResources()
    {
        if (dropItems == null || dropItems.Length == 0)
            return;

        foreach (DropItem item in dropItems)
        {
            if (item.prefab == null)
                continue;

            float randomValue =
                Random.Range(0f, 100f);

            if (randomValue <= item.dropChance)
            {
                int amount = Random.Range(
                    item.minAmount,
                    item.maxAmount + 1
                );

                for (int i = 0; i < amount; i++)
                {
                    Vector3 randomOffset =
                        Random.insideUnitSphere * 0.8f;

                    randomOffset.y = 0f;

                    Instantiate(
                        item.prefab,
                        transform.position + randomOffset,
                        Quaternion.identity
                    );
                }
            }
        }
    }
}

