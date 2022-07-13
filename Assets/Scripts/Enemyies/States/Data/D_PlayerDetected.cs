using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "newPlayerDetectedStateData", menuName = "Data/State Data/PlayerDetected State")]
public class D_PlayerDetected : ScriptableObject
{
    public float longRangeActionTime = 1.5f; // 敌人从发现玩家到冲向玩家的间隔时间(远程行为)
}
