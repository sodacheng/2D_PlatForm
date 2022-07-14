using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "newRangedAttackStateData", menuName = "Data/State Data/RangedAttack State Data")]
public class D_RangedAttackState : ScriptableObject
{
    [Header("远程攻击的射弹")]
    public GameObject projectile;
    [Header("射弹造成的伤害")]
    public float projectileDamage = 10f;
    [Header("射弹速度")]
    public float projectileSpeed = 12f;
    [Header("射弹飞行距离")]
    public float projectileTravelDistance = 5f;

}
