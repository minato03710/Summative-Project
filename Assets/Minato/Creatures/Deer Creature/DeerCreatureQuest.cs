using UnityEngine;

public class DeerCreatureQuest : MonoBehaviour
{

    // References

    [SerializeField] GameObject searchTrigger;
    public Animator deerAnimator;
    public FindFlower findFlower;

    void Awake()
    {
        deerAnimator = GetComponentInChildren<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

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
        }
    }

    void FoundFlowersCutscene()
    {
        Debug.Log("Cutscene should start");
        deerAnimator.SetBool("deerQuestComplete", true);
    }
}
