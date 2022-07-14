using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newMoveStateData", menuName = "Data/State Data/Move State")]
public class D_MoveState : ScriptableObject // 数据容器
{
    [Header("巡逻状态的移动速度")]
    public float movementSpeed = 3f;
}
