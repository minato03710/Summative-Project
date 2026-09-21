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
        if(Input.GetKey(KeyCode.A))
        {
            playerWalking = true;
        }

        if(Input.GetKey(KeyCode.D))
        {
            playerWalking = true;
        }

        Jumping();
    }

    void Jumping()
    {
        if (playerWalking)
        {
            playerAnimator.SetBool("isWalking", true);
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