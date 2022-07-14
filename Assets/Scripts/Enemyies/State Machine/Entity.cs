using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 所有敌人实例继承的基类脚本 包含了都拥有的数据,组件, 检测函数, 以及状态机的更新设置
/// </summary>
public class Entity : MonoBehaviour
{
    public FiniteStateMachine stateMachine;

    public D_Entity entityData; // 基础数据容器

    public int facingDirection { get; private set; } // 朝向方向

    // 所有 Enemy实体都拥有的
    public Rigidbody2D rb { get; private set; }
    public Animator anim { get; private set; }
    
    public GameObject aliveGO { get; private set; }
    public AnimationToStateMachine atsm { get; private set; } // 在我们的怪物中, 我们的Animator和脚本不在同一个GameObject上, 没法直接交互,所以我们将该脚本挂载Alive GameObject上 用于中继消息

    [SerializeField]
    private Transform wallCheck;
    [SerializeField]
    private Transform ledgeCheck; // 检测边沿是否要离开平台
    [SerializeField]
    private Transform playerCheck;
    [SerializeField]
    private Transform groundCheck; // 地面检测

    private float currentHealth;
    private float currentStunResistance;
    private float lastDamageTime;

    public int lastDamageDirection { get; private set; } // 受击方向

    private Vector2 velocityWorkSpace; // 用于设置速度时 使用的变量

    protected bool isStunned; // 是否被眩晕
    protected bool isDead;

    public virtual void Start()
    {
        facingDirection = 1;
        currentHealth = entityData.maxHealth;
        currentStunResistance = entityData.stunResistance;

        aliveGO = transform.Find("Alive").gameObject;
        rb = aliveGO.GetComponent<Rigidbody2D>();
        anim = aliveGO.GetComponent<Animator>();
        atsm = aliveGO.GetComponent<AnimationToStateMachine>();
        stateMachine = new FiniteStateMachine(); // 所有实体拥有他自己的状态机
    }

    public virtual void Update()
    {
        stateMachine.currentState.LogicUpdate();

        if(Time.time >= lastDamageTime + entityData.stunRecoveryTime)
        {
            ResetStunResistance();
        }
    }

    public virtual void FixedUpdate()
    {
        stateMachine.currentState.PhysicsUpdate();
    }

    /// <summary>
    /// 设置敌人的速度
    /// </summary>
    /// <param name="velocity"></param>
    public virtual void SetVelocity(float velocity)
    {
        velocityWorkSpace.Set(facingDirection * velocity, rb.velocity.y);
        rb.velocity = velocityWorkSpace;
    }

    public virtual void SetVelocity(float velocity, Vector2 angle, int direction)
    {
        angle.Normalize();
        velocityWorkSpace.Set(angle.x * velocity * direction, angle.y * velocity);
        rb.velocity = velocityWorkSpace;
    }

    /// <summary>
    /// 检测墙壁
    /// </summary>
    public virtual bool CheckWall()
    {
        return Physics2D.Raycast(wallCheck.position, aliveGO.transform.right, entityData.wallCheckDistance, entityData.whatIsGround);
    }

    /// <summary>
    /// 检测台沿(是否要离开平台了)
    /// </summary>
    public virtual bool CheckLedge()
    {
        return Physics2D.Raycast(ledgeCheck.position, Vector2.down, entityData.ledgeCheckDistance, entityData.whatIsGround);
    }

    public virtual bool CheckGround()
    {
        return Physics2D.OverlapCircle(groundCheck.position, entityData.groudCheckRadius, entityData.whatIsGround);
    }
    
    /// <summary>
    /// 检测玩家(近距离)
    /// </summary>
    /// <returns></returns>
    public virtual bool CheckPlayerInMinAgroRange()
    {
        return Physics2D.Raycast(playerCheck.position, aliveGO.transform.right, entityData.minAgroDistance, entityData.whatIsPlayer);
    }

    /// <summary>
    /// 检测玩家(远距离) - 最大仇恨距离
    /// </summary>
    /// <returns></returns>
    public virtual bool CheckPlayerInMaxAgroRange()
    {
        return Physics2D.Raycast(playerCheck.position, aliveGO.transform.right, entityData.maxAgroDistance, entityData.whatIsPlayer);
    }

    /// <summary>
    /// 检测玩家是否在怪物的近战攻击范围内
    /// </summary>
    /// <returns></returns>
    public virtual bool CheckPlayerInCloseRangeAction()
    {
        return Physics2D.Raycast(playerCheck.position, aliveGO.transform.right, entityData.closeRangeActionDistance, entityData.whatIsPlayer);
    }

    /// <summary>
    /// 重置眩晕抗性
    /// </summary>
    public virtual void ResetStunResistance()
    {
        isStunned = false;
        currentStunResistance = entityData.stunResistance;
    }
    /// <summary>
    /// 攻击将怪物弹起
    /// </summary>
    /// <param name="velocity"></param>
    public virtual void DamageHop(float velocity)
    {
        velocityWorkSpace.Set(rb.velocity.x, velocity);
        rb.velocity = velocityWorkSpace;
    }

    /// <summary>
    /// Damage函数
    /// </summary>
    public virtual void Damage(AttackDetails attackDetails)
    {
        lastDamageTime = Time.time; // 跟踪我们对敌人造成伤害的时间点

        currentHealth -= attackDetails.damageAmout; // 造成伤害
        currentStunResistance -= attackDetails.stunDamageAmount; // 计算剩余眩晕抗性

        DamageHop(entityData.damageHopSpeed);

        //生成击中特效
        Instantiate(entityData.hitParticle, aliveGO.transform.position, Quaternion.Euler(0f,0f,Random.Range(0.0f,360.0f)));

        if (attackDetails.position.x > aliveGO.transform.position.x)
        {
            lastDamageDirection = -1;
        }
        else
        {
            lastDamageDirection = 1;
        }

        if(currentStunResistance <= 0) // 当眩晕抗性小于等于0, 进入眩晕状态
        {
            isStunned = true;
        }

        if(currentHealth <= 0)
        {
            isDead = true;
        }
    }

    
    /// <summary>
    /// 翻转
    /// </summary>
    public virtual void Flip()
    {
        facingDirection *= -1;
        aliveGO.transform.Rotate(0f, 180f, 0f);
    }

    /// <summary>
    /// 让检测相关可视化
    /// </summary>
    public virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(wallCheck.position, wallCheck.position + (Vector3)(Vector2.right * facingDirection * entityData.wallCheckDistance));
        Gizmos.DrawLine(ledgeCheck.position, ledgeCheck.position + (Vector3)(Vector2.down * entityData.ledgeCheckDistance));

        Gizmos.DrawWireSphere(playerCheck.position + (Vector3)(Vector2.right * entityData.closeRangeActionDistance), 0.2f);
        Gizmos.DrawWireSphere(playerCheck.position + (Vector3)(Vector2.right * entityData.minAgroDistance), 0.2f);
        Gizmos.DrawWireSphere(playerCheck.position + (Vector3)(Vector2.right * entityData.maxAgroDistance), 0.2f);
    }
}
