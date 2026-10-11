using UnityEngine;

public class FindFlower : MonoBehaviour
{

    public int playerFlowerCount;
    public bool flowersCollected;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerFlowerCount = 0;
        flowersCollected = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerFlowerCount >= 3)
        {
            flowersCollected = true;
            Debug.Log("Ready to return to deer!");
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Flower"))
        {
            Debug.Log("Player a flower!");
            playerFlowerCount += 1;
            Debug.Log("Flowers: " + playerFlowerCount);
            Destroy(collision.gameObject);
        }

        else
        {
            Debug.Log("Flowers still need to be found");
        }
    }

}
