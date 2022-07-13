using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E1_PlayerDetectedState : PlayerDetectedState
{
    private Enemy1 enemy;
    public E1_PlayerDetectedState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_PlayerDetected stateData, Enemy1 enemy) : base(entity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // 切换状态
        if(performLongRangeAction) // 玩家还在怪物的最大仇恨范围内
        {
            stateMachine.ChangeState(enemy.chargeState);
        }
        else if(!isPlayerInMaxAgroRange) // 玩家不在怪物的仇恨范围内
        {
            stateMachine.ChangeState(enemy.lookForPlayerState);
        }

    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
