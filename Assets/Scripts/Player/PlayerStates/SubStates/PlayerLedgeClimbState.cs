using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLedgeClimbState : PlayerState
{
    private Vector2 detectedPos;
    private Vector2 cornerPos; // 平台角位置
    private Vector2 startPos;
    private Vector2 stopPos;

    private bool isHanging;
    private bool isClimbing;

    private int xInput;
    private int yInput;
    public PlayerLedgeClimbState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void AnimationTriiger()
    {
        base.AnimationTriiger();

        isHanging = true;
    }

    public override void Enter()
    {
        base.Enter();

        // 进入攀爬状态的时候 播放动画, 锁定角色的位置
        player.SetVelocityZero();
        player.transform.position = detectedPos;

        cornerPos = player.DetermineCornerPosition(); // 获取攀爬平台边角的位置坐标
        startPos.Set(cornerPos.x - (player.FacingDirection * playerData.startOffset.x), cornerPos.y - playerData.startOffset.y);
        stopPos.Set(cornerPos.x + (player.FacingDirection * playerData.stopOffset.x), cornerPos.y + playerData.stopOffset.y);

        player.transform.position = startPos;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        xInput = player.InputHandler.NormInputX;
        yInput = player.InputHandler.NormInputY;

        player.SetVelocityZero();
        player.transform.position = startPos;

        if(xInput == player.FacingDirection && isHanging && !isClimbing)
        {
            isClimbing = true;
            player.Anim.SetBool("climbLedge", true);
        }
        else if(yInput == -1 && isHanging && !isClimbing)
        {
            stateMachine.ChangeState(player.InAirState);
        }
    }

    public void SetDetectedPosition(Vector2 pos) => detectedPos = pos;



}
