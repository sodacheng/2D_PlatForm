using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "newIdleStateData", menuName = "Data/State Data/Idle State")]
public class D_IdleState : ScriptableObject
{
    [Header("最短和最长空闲时间,怪物进入Idle状态会处于之间的随机值")]
    public float minIdleTime = 1f;
    public float maxIdleTime = 2f;
}
