using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 RawMovementInput { get;private set; }
    public int NormInputX { get; private set; }
    public int NormInputY { get; private set; }
    public bool JumpInput { get;private set;}
    public bool JumptInputStop { get; private set; }

    [SerializeField]
    private float inputHoldTime = 0.2f;

    private float jumpInputStartTime;

    private void Update()
    {
        CheckJumpInputHoldTime();
    }

    // 编写函数来分配给 Player Input Event
    public void OnMoveInput(InputAction.CallbackContext context)
    {
        RawMovementInput = context.ReadValue<Vector2>();

        NormInputX = (int)(RawMovementInput * Vector2.right).normalized.x;
        NormInputY = (int)(RawMovementInput * Vector2.up).normalized.y;
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        // Jump - getbuttonDown
        if(context.started)
        {
            JumpInput = true;
            JumptInputStop = false;
            jumpInputStartTime = Time.time;
        }

        if(context.canceled)
        {
            JumptInputStop = true;
        }
    }

    public void UseJumpInput() => JumpInput = false;


    // 输入跳跃后会保持为True 直到我们使用它/超时
    private void CheckJumpInputHoldTime()
    {
        if (Time.time >= jumpInputStartTime + inputHoldTime) // 超时
        {
            JumpInput = false;
        }
    }
}
