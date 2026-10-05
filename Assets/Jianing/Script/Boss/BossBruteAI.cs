using System.Collections.Generic;
using UnityEngine;

public class BossBruteAI : BossBaseAI
{
    [Header("Movement and Timing")]
    public float moveSpeed = 2.1f;
    public float stoppingDistance = 1.8f;

    [Min(0.1f)] public float attackInterval = 3f;
    [Min(0f)] public float windupDuration = 0.4f;
    [Min(0f)] public float recoveryDuration = 0.5f;

    [Header("Normal - Forward Cone")]
    public float normalDamage = 25f;
    public float normalRange = 2.5f;

    [Range(1f, 180f)] public float normalAngle = 100f;

    [Header("Leap and Stomp")]
    public float stompDamage = 35f;
    public float stompRadius = 3f;
    public float maxJumpDistance = 5f;
    public float jumpHeight = 2f;

    [Min(0.1f)] public float jumpDuration = 0.8f;

    [Header("Dash")]
    public float dashDamage = 30f;
    [Min(0.1f)] public float dashSpeed = 12f;
    public float dashDistance = 7f;
    public float dashHitRadius = 0.9f;

    private enum State
    {
        Chase,
        Windup,
        Jump,
        Landing,
        Dash,
        Recovery
    }

    private State state;
    private int chosenAttack;
    private bool engaged;

    private double nextAttack;
    private double stateStarted;

    private Vector3 direction;
    private Vector3 jumpStart;
    private Vector3 jumpEnd;

    private float dashTravelled;

    private readonly HashSet<PlayerHealth> dashHits =
        new HashSet<PlayerHealth>();

    protected override void TickAI()
    {
        double now = Time.timeAsDouble;

        if (!engaged)
        {
            if (ToPlayer().magnitude > detectionRange)
            {
                MoveGround(Vector3.zero);
                return;
            }

            engaged = true;
            nextAttack = now + Mathf.Max(0.1f, attackInterval);
        }

        switch (state)
        {
            case State.Chase:
                if (now >= nextAttack && controller.isGrounded)
                {
                    chosenAttack = Random.Range(0, 3);
                    direction = ToPlayer().normalized;

                    if (direction.sqrMagnitude < 0.001f)
                        direction = transform.forward;

                    Face(direction);

                    // 在准备动作开始时锁定玩家位置。
                    jumpEnd = player.position;

                    nextAttack =
                        now + Mathf.Max(0.1f, attackInterval);

                    Enter(State.Windup);
                    MoveGround(Vector3.zero);
                }
                else
                {
                    Vector3 delta = ToPlayer();
                    Face(delta);

                    MoveGround(
                        delta.magnitude > stoppingDistance
                            ? delta.normalized * moveSpeed
                            : Vector3.zero);
                }
                break;

            case State.Windup:
                MoveGround(Vector3.zero);

                if (now - stateStarted >= windupDuration)
                    BeginAttack();

                break;

            case State.Jump:
                float t = Mathf.Clamp01(
                    (float)(now - stateStarted) /
                    Mathf.Max(0.1f, jumpDuration));

                Vector3 position =
                    Vector3.Lerp(jumpStart, jumpEnd, t) +
                    Vector3.up *
                    (Mathf.Sin(t * Mathf.PI) *
                     Mathf.Max(0f, jumpHeight));

                CollisionFlags flags =
                    controller.Move(position - transform.position);

                if (t >= 1f ||
                    (flags & CollisionFlags.Above) != 0)
                {
                    verticalSpeed = -2f;
                    Enter(State.Landing);
                }
                break;

            case State.Landing:
                CollisionFlags landed =
                    MoveGround(Vector3.zero);

                if (controller.isGrounded ||
                    (landed & CollisionFlags.Below) != 0)
                {
                    DamageArea(
                        transform.position + Vector3.up * 0.5f,
                        stompRadius,
                        stompDamage);

                    Enter(State.Recovery);
                }
                break;

            case State.Dash:
                Dash();
                break;

            case State.Recovery:
                MoveGround(Vector3.zero);

                if (now - stateStarted >= recoveryDuration)
                    Enter(State.Chase);

                break;
        }
    }

    private void BeginAttack()
    {
        if (chosenAttack == 0)
        {
            // 普通攻击：前方扇形。
            DamageArea(
                transform.position + Vector3.up,
                normalRange,
                normalDamage,
                null,
                Mathf.Cos(normalAngle * 0.5f * Mathf.Deg2Rad));

            Enter(State.Recovery);
        }
        else if (chosenAttack == 1)
        {
            // 跳跃：朝准备时锁定的位置移动一小段距离。
            jumpStart = transform.position;

            Vector3 delta = jumpEnd - jumpStart;
            delta.y = 0f;

            float distance = Mathf.Min(
                delta.magnitude,
                Mathf.Max(0f, maxJumpDistance));

            distance = ClearDistance(direction, distance);

            Vector3 candidate =
                jumpStart + direction * distance;

            float footOffset =
                controller.height * 0.5f -
                controller.center.y;

            jumpEnd = TryGround(
                candidate,
                controller.radius,
                controller.height,
                obstacleLayers,
                out Vector3 ground)
                    ? ground + Vector3.up * footOffset
                    : jumpStart;

            Enter(State.Jump);
        }
        else
        {
            dashTravelled = 0f;
            dashHits.Clear();
            Enter(State.Dash);
        }
    }

    private void Dash()
    {
        float wanted = Mathf.Min(
            Mathf.Max(0.1f, dashSpeed) * Time.deltaTime,
            Mathf.Max(0f, dashDistance - dashTravelled));

        float allowed =
            ClearDistance(direction, wanted);

        Vector3 before = transform.position;

        Vector3 velocity = Time.deltaTime > 0f
            ? direction * allowed / Time.deltaTime
            : Vector3.zero;

        CollisionFlags flags = MoveGround(velocity);

        Vector3 after = transform.position;
        Vector3 travelled = after - before;
        travelled.y = 0f;

        dashTravelled += travelled.magnitude;

        // 每次冲刺对同一个玩家只造成一次伤害。
        DamagePath(
            before + Vector3.up,
            after + Vector3.up,
            dashHitRadius,
            dashDamage,
            dashHits);

        if (allowed < wanted ||
            wanted <= 0.001f ||
            travelled.magnitude < 0.001f ||
            (flags & CollisionFlags.Sides) != 0 ||
            dashTravelled >= dashDistance - 0.01f)
        {
            Enter(State.Recovery);
        }
    }

    private void Enter(State next)
    {
        state = next;
        stateStarted = Time.timeAsDouble;
    }

    protected override void ResetAttack()
    {
        dashHits.Clear();

        nextAttack =
            Time.timeAsDouble +
            Mathf.Max(0.1f, attackInterval);

        Enter(State.Chase);
    }
}