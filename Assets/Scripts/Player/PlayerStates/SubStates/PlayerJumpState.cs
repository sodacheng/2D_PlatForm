using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerAbilityState
{
    private int amountOfjumpsLeft;

    public PlayerJumpState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
        amountOfjumpsLeft = playerData.amountOfJumps;
    }

    public override void Enter()
    {
        base.Enter();

        player.SetVelocityY(playerData.jumpVelocity);

        isAbilityDone = true; // 跳跃进入之后就进让AbilityState来接管

        DecreaseAmoutOfJumpsLeft();
        player.InAirState.SetIsJumping();
    }

    public bool CanJump()
    {
        if(amountOfjumpsLeft > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ResetAmountOfJumpsLeft() => amountOfjumpsLeft = playerData.amountOfJumps;

    public void DecreaseAmoutOfJumpsLeft() => amountOfjumpsLeft--;
}
