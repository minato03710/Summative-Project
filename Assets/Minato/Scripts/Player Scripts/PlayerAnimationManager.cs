using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{

    public Animator playerAnimator;
    private bool playerWalking;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerAnimator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        while(Input.GetKey(KeyCode.LeftArrow))
        {
            playerWalking = true;
        }

        while(Input.GetKey(KeyCode.RightArrow))
        {
            playerWalking = true;
        }

        Jumping();
    }

    void Jumping()
    {
        if (playerWalking)
        {
            playerAnimator.SetBool("isWalking", false);
            playerWalking = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            playerAnimator.SetBool("onGround", true);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            playerAnimator.SetBool("onGround", false);
        }
    }

}