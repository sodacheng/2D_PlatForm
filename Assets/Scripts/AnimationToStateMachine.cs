using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 在我们的怪物中, 我们的Animator和脚本不在同一个GameObject上, 没法直接交互,所以我们将该脚本挂载Alive GameObject上 用于中继消息
/// </summary>
public class AnimationToStateMachine : MonoBehaviour
{
    public AttackState attackState; // 引用AttackState 该脚本被Entity获取
    private void TriggerAttack()
    {
        attackState.TriggerAttack();
    }

    private void FinishAttack()
    {
        attackState.FinishAttack();
    }
}
