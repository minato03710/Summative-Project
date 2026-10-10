using UnityEngine;

public class BossAnimationManager : MonoBehaviour
{

    private CharacterController bossCharacterController;
    public Animator bossAnimator;
    public BossBruteAI bossBruteAI;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bossCharacterController = GetComponent<CharacterController>();
        bossAnimator = GetComponentInChildren<Animator>();
        bossAnimator.SetBool("bossIsMoving", true);
        bossAnimator.SetBool("bossInAir", false);
    }

    // Update is called once per frame
    void Update()
    {

        if (bossBruteAI.bossPlayMoving == false)
        {
            bossAnimator.SetBool("bossIsMoving", false);
            bossAnimator.SetBool("bossInAir", false);
        }

        if (bossBruteAI.bossPlayMoving == true)
        {
            bossAnimator.SetBool("bossIsMoving", true);
        }

        // if () Attacking
        {
            //bossAnimator.SetBool("bossIsAttacking", true);
        }

        // if () In air
        {
            //bossAnimator.SetBool("bossInAir", true);
        }

        if (bossBruteAI.bossPlayDash == true)
        {
            bossAnimator.SetBool("bossIsDashing", true);
        }

        // if () Summoning
        {
            //bossAnimator.SetBool("bossIsSummoning", true);
        }

    }
}
