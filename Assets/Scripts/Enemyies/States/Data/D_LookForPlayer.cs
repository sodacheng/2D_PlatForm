using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "newLookForPlayerStateData", menuName = "Data/State Data/Look For Player State")]
public class D_LookForPlayer : ScriptableObject
{
    [Header("丢失玩家目标后转头寻找玩家的次数")]
    public int amountOfturns = 2; // 敌人巡逻寻找玩家的圈数(转向次数)
    [Header("转头的间隔时间")]
    public float timeBetweenTurns = 0.75f; //每次转向的间隔时间  
}
