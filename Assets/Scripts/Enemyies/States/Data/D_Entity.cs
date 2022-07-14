using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newEntityData", menuName ="Data/Entity Data/Base Data")]
public class D_Entity : ScriptableObject
{
    [Header("最大血量")]
    public float maxHealth = 30f;
    [Header("击退上升速度")]
    public float damageHopSpeed = 10f;

    [Header("墙壁检测距离")]
    public float wallCheckDistance = 0.2f;
    [Header("平台边缘检测深度")]
    public float ledgeCheckDistance = 0.4f;
    [Header("球形地面检测半径")]
    public float groudCheckRadius = 0.3f;
    [Header("最小仇恨距离")]
    public float minAgroDistance = 3f; // 检测到玩家 产生仇恨的范围
    [Header("最大仇恨距离")]
    public float maxAgroDistance = 4f; // 玩家逃离, 取消仇恨状态的最大范围

    [Header("眩晕抗性 - 承受多少次伤害才晕")]
    public float stunResistance = 3f;
    [Header("多长时间不被攻击将重置眩晕抗性")]
    public float stunRecoveryTime = 2f;
    [Header("近战攻击检测距离")]
    public float closeRangeActionDistance = 1f; //近战的攻击检测距离

    [Header("击中效果特效")]
    public GameObject hitParticle;
    [Header("地面层")]
    public LayerMask whatIsGround;
    [Header("玩家层")]
    public LayerMask whatIsPlayer;
}
