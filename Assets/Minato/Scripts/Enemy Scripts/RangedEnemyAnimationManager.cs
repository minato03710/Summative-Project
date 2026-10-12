using UnityEngine;
using System.Collections;

public class RangedEnemyAnimationManager : MonoBehaviour
{

    // References

    public Animator rangedAnimator;
    private CharacterController rangedCharacterController;
    public RangedEnemyAI rangedScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rangedCharacterController = GetComponent<CharacterController>();
        rangedAnimator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rangedScript.rangedPlayAttack == true)
        {
            rangedAnimator.SetBool("rangedIsAttacking", true);
        }

        if (rangedScript.rangedPlayAttack == false)
        {
            rangedAnimator.SetBool("rangedIsAttacking", false);
        }
    }

}