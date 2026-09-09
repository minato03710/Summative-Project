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

    protected bool playerDetected;

    protected virtual void Start()
    {
        // Auto find Player
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    protected virtual void Update()
    {
        if (player == null)
            return;

        // Player is within detection range
        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        if (distance <= detectionRange)
        {
            playerDetected = true;
        }
        else
        {
            playerDetected = false;
        }

        if (playerDetected)
        {
            MoveTowardsPlayer();
        }
    }

    protected virtual void MoveTowardsPlayer()
    {
        if (player == null)
            return;

        Vector3 direction =
            player.position - transform.position;

        direction.y = 0;

        float distance = direction.magnitude;

        // Close enough to the player
        if (distance <= stoppingDistance)
            return;

        direction.Normalize();

        transform.position +=
            direction * moveSpeed * Time.deltaTime;

        // Faceing player
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                10f * Time.deltaTime
            );
        }
    }
}


