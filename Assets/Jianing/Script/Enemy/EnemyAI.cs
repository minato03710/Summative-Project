using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

[Header("Movement")]
    public float moveSpeed = 3f;
    public float stoppingDistance = 1.5f;

    [Header("Detection")]
    public float detectionRange = 10f;

    [Header("Enemy Separation")]
    public float separationRadius = 1.5f;
    public float separationStrength = 2f;

    [Header("Gravity")]
    public float gravity = -20f;

    protected bool playerDetected;

    // 只在父类定义一次
    protected CharacterController controller;

    private float verticalVelocity;

    protected virtual void Start()
    {
        controller =
            GetComponent<CharacterController>();

        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    protected void ApplyGravity()
    {
        if (controller == null)
            return;

        if (controller.isGrounded)
        {
            if (verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }
        }
        else
        {
            verticalVelocity +=
                gravity * Time.deltaTime;
        }

        controller.Move(
            Vector3.up *
            verticalVelocity *
            Time.deltaTime
        );
    }

    protected Vector3 CalculateSeparation()
    {
        Vector3 separation =
            Vector3.zero;

        Collider[] nearbyObjects =
            Physics.OverlapSphere(
                transform.position,
                separationRadius
            );

        foreach (Collider other in nearbyObjects)
        {
            if (other.gameObject == gameObject)
                continue;

            if (!other.CompareTag("Enemy"))
                continue;

            Vector3 difference =
                transform.position -
                other.transform.position;

            difference.y = 0f;

            float distance =
                difference.magnitude;

            if (distance <= 0.01f)
                continue;

            float strength =
                1f -
                Mathf.Clamp01(
                    distance /
                    separationRadius
                );

            separation +=
                difference.normalized *
                strength;
        }

        return separation;
    }


}





