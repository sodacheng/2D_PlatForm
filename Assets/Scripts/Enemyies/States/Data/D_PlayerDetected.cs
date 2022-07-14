using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "newPlayerDetectedStateData", menuName = "Data/State Data/PlayerDetected State")]
public class D_PlayerDetected : ScriptableObject
{
    [Header("longRangeAction的间隔时间")]
    public float longRangeActionTime = 1.5f; // 敌人从发现玩家到---冲向---> 攻击(比如射箭) 玩家的间隔时间(远程行为)
}
