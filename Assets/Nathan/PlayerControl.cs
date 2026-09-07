using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Data;

public class PlayerControl : MonoBehaviour
{
    public CharacterController characterController;
    public float walkSpeed;
    public float sprintSpeed;
    public float jumpPower;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }


    void Jump()
    {

    }

    void Move()
    {
        float horizontalMove = Input.GetAxis("Horizontal");
        float verticalMove = Input.GetAxis("Vertical");

        Vector3 move = transform.forward * verticalMove + transform.right * horizontalMove;
        characterController.Move(walkSpeed * Time.deltaTime * move);
    }
}
