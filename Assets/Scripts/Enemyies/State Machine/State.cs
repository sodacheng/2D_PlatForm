using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 声明需要跟踪的所有变量
/// </summary>
public class State
{
    protected FiniteStateMachine stateMachine;
    protected Entity entity; // 声明他属于哪个实体

    protected float startTime;

    protected string animBoolName; // 我们不必关心动画设置, 只需要保证参数和状态名匹配

    public State(Entity entity, FiniteStateMachine stateMachine, string animBoolName)
    {
        this.entity = entity;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }

    /// <summary>
    /// 需要跟踪状态的开始时间
    /// </summary>
    public virtual void Enter()
    {
        startTime = Time.time;
        entity.anim.SetBool(animBoolName, true);
        DoChecks();
    }

    public virtual void Exit()
    {
        entity.anim.SetBool(animBoolName, false);
    }

    public virtual void LogicUpdate()
    {

    }

    public virtual void PhysicsUpdate()
    {
        DoChecks();
    }


    /// <summary>
    /// 对周围环境的检测,持续更新设置的相关Bool值, 会在Enter() 和 PhysicsUpdate()中调用
    /// </summary>
    public virtual void DoChecks()
    {

    }

}
