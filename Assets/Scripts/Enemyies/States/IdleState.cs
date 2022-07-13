using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{
    protected D_IdleState stateData;

    protected bool flipAfterIdle; // 在离开空闲状态时 是否要转身
    protected bool isIdleTimeOver; // 要空闲状态结束
    protected bool isPlayerInMinAgroRange;

    protected float idleTime;
    public IdleState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_IdleState stateData) : base(entity, stateMachine, animBoolName)
    {
        this.stateData = stateData; 
    }

    public override void DoChecks()
    {
        base.DoChecks();
        isPlayerInMinAgroRange = entity.CheckPlayerInMinAgroRange();
    }



    /// <summary>
    /// 当进入空闲状态时, 将在随机的时间后选择要进入的状态
    /// </summary>
    public override void Enter()
    {
        base.Enter();

        // 进入Idle状态时, 希望敌人停止移动
        entity.SetVelocity(0f);
        isIdleTimeOver = false;
        SetRandomIdleTime();
    }

    public override void Exit()
    {
        base.Exit();

        if(flipAfterIdle)
        {
            entity.Flip();
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (Time.time >= startTime + idleTime) // startTime是在State中声明的进入状态的开始时间
        {
            isIdleTimeOver = true;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public void SetFlipAfterIdle(bool flip)
    {
        flipAfterIdle = flip;
    }

    /// <summary>
    /// 设置随机时长的空闲状态持续时间, 范围由D_IdleState设置
    /// </summary>
    private void SetRandomIdleTime()
    {
        idleTime = Random.Range(stateData.minIdleTime, stateData.maxIdleTime);
    }
}
