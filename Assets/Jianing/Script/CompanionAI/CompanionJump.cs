using UnityEngine;

public class CompanionJump : MonoBehaviour
{
    [Header("Jump")]
    public float jumpForce = 7f;
    public float jumpCooldown = 1f;


[Header("Obstacle Detection")]
    public float checkDistance = 1.5f;
    public float checkHeight = 0.8f;
    public LayerMask obstacleLayer;

    private CompanionMovement movement;

    private float jumpTimer;

    void Start()
    {
        movement =
            GetComponent<CompanionMovement>();

        if (movement == null)
        {
            Debug.LogError(
                "CompanionJump 找不到 CompanionMovement!"
            );
        }
    }

    void Update()
    {
        if (jumpTimer > 0f)
        {
            jumpTimer -= Time.deltaTime;
        }

        CheckObstacle();
    }

    void CheckObstacle()
    {
        if (movement == null)
            return;

        // 检查是否在地面
        if (!movement.IsGrounded())
        {
            return;
        }

        if (jumpTimer > 0f)
            return;

        Vector3 direction =
            movement.GetMoveDirection();

        if (direction.sqrMagnitude < 0.01f)
            return;

        direction.Normalize();

        Vector3 origin =
            transform.position +
            Vector3.up * checkHeight;

        RaycastHit hit;

        bool hitObstacle =
            Physics.Raycast(
                origin,
                direction,
                out hit,
                checkDistance,
                obstacleLayer
            );

        if (!hitObstacle)
            return;

        Debug.Log(
            "Companion 检测到障碍物：" +
            hit.collider.gameObject.name
        );

        Debug.Log(
            "Companion Grounded = " +
            movement.IsGrounded()
        );

        // 检查障碍物上方
        Vector3 upperOrigin =
            transform.position +
            Vector3.up * 1.8f;

        bool blockedAbove =
            Physics.Raycast(
                upperOrigin,
                direction,
                checkDistance,
                obstacleLayer
            );

        if (blockedAbove)
        {
            Debug.Log(
                "上方被挡住，不能跳!"
            );

            return;
        }

        Debug.Log(
            "Companion 准备跳跃!"
        );

        movement.Jump(jumpForce);

        jumpTimer =
            jumpCooldown;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Vector3 direction =
            transform.forward;

        if (Application.isPlaying)
        {
            CompanionMovement move =
                GetComponent<CompanionMovement>();

            if (move != null &&
                move.GetMoveDirection().sqrMagnitude > 0.01f)
            {
                direction =
                    move.GetMoveDirection();
            }
        }

        Vector3 origin =
            transform.position +
            Vector3.up * checkHeight;

        Gizmos.DrawRay(
            origin,
            direction.normalized *
            checkDistance
        );
    }


}
