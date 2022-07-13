using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E1_ChargeState : ChargeState
{
    private Enemy1 enemy;
    public E1_ChargeState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_ChargeState stateData, Enemy1 enemy) : base(entity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
    }

    public override void DoChecks()
    {
        base.DoChecks();
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
        if (performCloseRangeAction)
        {
            stateMachine.ChangeState(enemy.meleeAttackState);
        }
        else if (!isDetectingLedge || isDetectingWall) // 冲刺到撞墙/平台边缘, 转向寻找玩家的状态
        {
            stateMachine.ChangeState(enemy.lookForPlayerState);
        }

        else if (isChargeTimeOver) // 一次冲向玩家的行为结束
        {
            if (isPlayerInMinAgroRange) // 如果玩家还在最小仇恨范围内
            {
                stateMachine.ChangeState(enemy.playerDetectedState); // 检测到玩家在最小仇恨的状态
            }
            else
            {
                stateMachine.ChangeState(enemy.lookForPlayerState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
