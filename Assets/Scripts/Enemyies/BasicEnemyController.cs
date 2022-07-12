using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyController : MonoBehaviour
{
    private enum State
    {
        Moving,
        KnockBack,
        Dead
    }

    private State currentState;

    [SerializeField]
    private float
        groundCheckDistance,
        wallCheckDistance,
        movementSpeed,
        maxHealth,
        knockbackDuration;

    private bool
        groundDetected, // 地面检测
        wallDetected; // 墙壁检测

    private GameObject alive; // Enmey对象
    private Rigidbody2D aliveRb;
    private Animator aliveAnim;

    private int
        facingDirection, // 面向方向 右正1左负1
        damageDirection; // 伤害来源方向


    private float
        currentHealth,
        knockbackStartTime;

    private Vector2 movement;

    [SerializeField]
    private Transform
        groundCheck,
        wallCheck;

    [SerializeField]
    private LayerMask whatIsGround;

    [SerializeField]
    private Vector2 knockbackSpeed;

    [SerializeField]
    private GameObject hitParticle, deathChunkParticle, deathBloodParticle;

    private void Start()
    {
        alive = transform.Find("Alive").gameObject;
        aliveRb = alive.GetComponent<Rigidbody2D>();
        facingDirection = 1;
        aliveAnim = alive.GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    /// <summary>
    /// 将确定哪个状态当前处于活动状态 (switch)
    /// </summary>
    private void Update()
    {
        switch (currentState)
        {
            case State.Moving:
                UpdateMovingState();
                break;
            case State.KnockBack:
                UpdateKnockBackState();
                break;
            case State.Dead:
                UpdateDeadkState();
                break;
        }
    }

    // Moving State -------------------------------------------------------------------

    private void EnterMovingState()
    {

    }

    private void UpdateMovingState()
    {
        groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        wallDetected = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);

        if (!groundDetected || wallDetected)
        {
            Flip();
        }
        else
        {
            //Move
            movement.Set(movementSpeed * facingDirection, aliveRb.velocity.y);
            aliveRb.velocity = movement;
        }
    }

    private void ExitMovingState()
    {

    }

    // Knockback State -------------------------------------------------------------------
    private void EnterKnockbackState()
    {
        knockbackStartTime = Time.time;
        movement.Set(knockbackSpeed.x * damageDirection, knockbackSpeed.y);
        aliveRb.velocity = movement;
        aliveAnim.SetBool("knockback", true);
    }

    private void UpdateKnockBackState()
    {
        if (Time.time >= knockbackStartTime + knockbackDuration)
        {
            SwitchState(State.Moving);
        }
    }

    private void ExitKnockbackState()
    {
        aliveAnim.SetBool("knockback", false);
    }

    // Dead State -------------------------------------------------------------------
    private void EnterDeadState()
    {
        // 实例化血液粒子 和 碎块
        Instantiate(deathChunkParticle,alive.transform.position, deathChunkParticle.transform.rotation);
        Instantiate(deathBloodParticle,alive.transform.position, deathBloodParticle.transform.rotation);

        Destroy(gameObject);
    }

    private void UpdateDeadkState()
    {

    }

    private void ExitDeadState()
    {

    }

    // Other Functions -------------------------------------------------------------------
    private void Damage(float[] attackDetails) // sendmessage 只允许我们发送一个参数 我们将发送伤害 和我们的x位置, 让我们知道敌人站在我们的哪一边
    {
        currentHealth -= attackDetails[0];

        Instantiate(hitParticle, alive.transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360)));

        if (attackDetails[1] > alive.transform.position.x)
        {
            damageDirection = -1;
        }
        else
        {
            damageDirection = 1;
        }


        // 实例化Hit Particle
        if (currentHealth > 0.0f)
        {
            SwitchState(State.KnockBack);
        }
        else if (currentHealth <= 0.0f)
        {
            SwitchState(State.Dead);
        }
    }


    private void Flip()
    {
        facingDirection *= -1;
        alive.transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    private void SwitchState(State state)
    {
        // 退出当前状态
        switch (currentState)
        {
            case State.Moving:
                ExitMovingState();
                break;
            case State.KnockBack:
                ExitKnockbackState();
                break;
            case State.Dead:
                ExitDeadState();
                break;
        }

        // 进入新状态
        switch (state)
        {
            case State.Moving:
                EnterMovingState();
                break;
            case State.KnockBack:
                EnterKnockbackState();
                break;
            case State.Dead:
                EnterDeadState();
                break;
        }

        currentState = state;
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
    }
}
