using UnityEngine;

public class Enemy_Spawner : MonoBehaviour
{

    public GameObject SpawnerParent;
    public GameObject EnemyType;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnEnemy();
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void SpawnEnemy()
    {
        GameObject newChild = Instantiate(EnemyType, SpawnerParent.transform);
        newChild.transform.localPosition = Vector3.zero;
    }


    private void OnTransformChildrenChanged()
    {
        if (transform.childCount == 0)
        {
            SpawnEnemy();
        }

    }

}
