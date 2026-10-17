using UnityEngine;

public class DeerCreatureQuest : MonoBehaviour
{

    // References

    [SerializeField] GameObject searchTrigger;
    public Animator deerAnimator;
    public FindFlower findFlower;
    public GameObject caveBarrier;
    public GameObject playerReference;

    void Awake()
    {
        deerAnimator = GetComponentInChildren<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        caveBarrier = GameObject.Find("Cave Barrier");
        playerReference = GameObject.Find("Player Animated");
        findFlower = playerReference.GetComponent<FindFlower>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider collision)
    {
        if (findFlower.flowersCollected == true)
        {
            searchTrigger.SetActive(false);
            FoundFlowersCutscene();
            Destroy(caveBarrier);
        }
    }

    void FoundFlowersCutscene()
    {
        Debug.Log("Cutscene should start");
        deerAnimator.SetBool("deerQuestComplete", true);
    }
}
