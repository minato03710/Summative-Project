using UnityEngine;

public class DeerCreatureQuest : MonoBehaviour
{

    // References

    [SerializeField] GameObject searchTrigger;
    public Animator deerAnimator;
    public int flowerCount;
    public DeerFlower deerFlower;

    void Awake()
    {
        deerAnimator = GetComponentInChildren<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        flowerCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Flowers:" + flowerCount);
        // Flower check
        if (deerFlower.newFlower == true)
        {
            Debug.Log("Found flower!");
            flowerCount += 1;
            deerFlower.newFlower = false;
            Debug.Log("Flowers:" + flowerCount);
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        if (flowerCount >= 3)
        {
            Debug.Log("Player found all 3 flowers!");
            searchTrigger.SetActive(false);
            FoundFlowersCutscene();
        }

        else
        {
            Debug.Log("Flowers still need to be found");
        }
    }

    void FoundFlowersCutscene()
    {
        Debug.Log("Cutscene should start");
    }
}
