using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newMeleeAttackStateData", menuName = "Data/State Data/Melee Attack State Data")]
public class D_MeleeAttack : ScriptableObject
{
    [Header("近战攻击的半径范围")]
    public float attackRadius = 0.5f;
    [Header("近战攻击的伤害")]
    public float attackDamage = 10f;
    [Header("玩家层")]
    public LayerMask whatIsPlayer;
}
