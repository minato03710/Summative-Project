using UnityEngine;
using System.Collections;

public class BeetleAnimationManager : MonoBehaviour
{

    // References

    public Animator beetleAnimator;
    private CharacterController beetleCharacterController;
    public EnemyAI beetleScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        beetleCharacterController = GetComponent<CharacterController>();
        beetleAnimator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // if (beetleEnemyScript)
        {
            BeetleAttack();
        }
    }

    void BeetleAttack()
    {
        StartCoroutine(BeetleAnimationTime());
    }

    IEnumerator BeetleAnimationTime()
        {
            // beetleAnimator.SetBool
            yield return new WaitForSeconds(3f);
            // beetleAnimator.SetBool
            yield return null;
        }

}