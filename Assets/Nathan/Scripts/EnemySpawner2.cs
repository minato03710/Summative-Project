using UnityEngine;

public class EnemySpawner2 : MonoBehaviour
{
    public GameObject enemyPrefab;

    public float spawnInterval = 10f;

    void Start()
    {
        InvokeRepeating(
            nameof(SpawnEnemy),
            1f,
            spawnInterval);
    }

    void SpawnEnemy()
    {
        Instantiate(
            enemyPrefab,
            transform.position,
            Quaternion.identity);
    }
}
