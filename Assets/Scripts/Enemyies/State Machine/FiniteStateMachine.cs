using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 有限状态机FSM
/// </summary>
public class FiniteStateMachine
{
    // 跟踪当前状态
    public State currentState { get; private set; }

    /// <summary>
    /// 初始化函数
    /// </summary>
    /// <param name="startingState"></param>
    public void Initialize(State startingState)
    {
        currentState = startingState;
        currentState.Enter();
    }

    public void ChangeState(State newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }


}
