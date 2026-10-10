using UnityEngine;

public class RangedEnemyAnimationManager : MonoBehaviour
{

    public Animator rangedEnemyAnimator;
    private CharacterController characterController;
    public RangedEnemyAI rangedEnemyScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        rangedEnemyAnimator = GetComponentInChildren<Animator>(); // References animator from visuals
    }

    // Update is called once per frame
    void Update()
    {
        // if (rangedEnemyScript.)
        {
            rangedEnemyAnimator.SetBool("rangedIsAttacking", true);
        }
    }
}
