using UnityEngine;

public class BossAnimationManager : MonoBehaviour
{

    public Animator bossAnimator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bossAnimator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // if () Moving
        {
            bossAnimator.SetBool("bossIsMoving", true);
        }

        // if () Attacking
        {
            bossAnimator.SetBool("bossIsAttacking", true);
        }

        // if () In air
        {
            bossAnimator.SetBool("bossInAir", true);
        }

        // if () Dashing
        {
            bossAnimator.SetBool("bossIsDashing", true);
        }

        // if () Summoning
        {
            bossAnimator.SetBool("bossIsSummoning", true);
        }

    }
}
