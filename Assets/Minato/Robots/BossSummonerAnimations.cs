using UnityEngine;

public class BossSummonerAnimations : MonoBehaviour
{

    private CharacterController bossCharacterController;
    public Animator bossAnimator;
    public BossSummonerAI bossSummonerAI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bossCharacterController = GetComponent<CharacterController>();
        bossAnimator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveCheck();
    }

    void MoveCheck()
    {
        if (bossSummonerAI.bossPlayMoving == false)
        {
            bossAnimator.SetBool("bossIsMoving", false);
        }

        if (bossSummonerAI.bossPlayMoving == true)
        {
            bossAnimator.SetBool("bossIsMoving", true);
        }
    }
}
