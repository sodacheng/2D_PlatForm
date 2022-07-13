using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E1_LookForPlayerState : LookForPlayerState
{
    private Enemy1 enemy;
    public E1_LookForPlayerState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_LookForPlayer stateData, Enemy1 enemy) : base(entity, stateMachine, animBoolName, stateData)
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

        if (isPlayerInMinAgroRange) // 发现了玩家在最小仇恨范围内
        {
            stateMachine.ChangeState(enemy.playerDetectedState);
        }
        else if(isAllTurnsTimeDonw) // 警戒巡逻时间结束(没有找到玩家)
        {
            stateMachine.ChangeState(enemy.moveState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
