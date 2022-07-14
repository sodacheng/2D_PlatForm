using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "newChargeStateData", menuName = "Data/State Data/Charge State")]
public class D_ChargeState : ScriptableObject
{
    [Header("冲向玩家的速度")]
    public float chargeSpeed = 6f; // 当敌人发现玩家并冲向玩家的时候, 会有一个与Move时不同的速度
    [Header("一次冲刺的持续时间")]
    public float chargeTime = 2f; // 向玩家冲刺状态的持续时间
}
