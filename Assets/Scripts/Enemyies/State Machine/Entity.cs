using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 所有敌人实例继承的基类脚本 包含了都拥有的数据,组件, 检测函数, 以及状态机的更新设置
/// </summary>
public class Entity : MonoBehaviour
{
    public FiniteStateMachine stateMachine;

    public D_Entity entityData; // 数据容器

    public int facingDirection { get; private set; } // 朝向方向

    // 所有 Enemy实体都拥有的
    public Rigidbody2D rb { get; private set; }
    public Animator anim { get; private set; }
    
    public GameObject aliveGO { get; private set; }

    [SerializeField]
    private Transform wallCheck;
    [SerializeField]
    private Transform ledgeCheck;
    [SerializeField]
    private Transform playerCheck;

    private Vector2 velocityWorkSpace; // 用于设置速度时 使用的变量

    public virtual void Start()
    {
        facingDirection = 1;
        aliveGO = transform.Find("Alive").gameObject;
        rb = aliveGO.GetComponent<Rigidbody2D>();
        anim = rb.GetComponent<Animator>();

        stateMachine = new FiniteStateMachine(); // 所有实体拥有他自己的状态机
    }

    public virtual void Update()
    {
        stateMachine.currentState.LogicUpdate();
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

    /// <summary>
    /// 检测墙壁
    /// </summary>
    public virtual bool CheckWall()
    {
        return Physics2D.Raycast(wallCheck.position, aliveGO.transform.right, entityData.wallCheckDistance, entityData.whatIsGround);
    }

    /// <summary>
    /// 检测台沿
    /// </summary>
    public virtual bool CheckLedge()
    {
        return Physics2D.Raycast(ledgeCheck.position, Vector2.down, entityData.ledgeCheckDistance, entityData.whatIsGround);
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
    }
}
