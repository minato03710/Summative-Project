using UnityEngine;

public class CompanionAnimationManager : MonoBehaviour
{

    public Animator playerAnimator;
    private bool playerWalking;
    private CharacterController characterController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerAnimator = GetComponentInChildren<Animator>(); // References animator from player visuals
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
