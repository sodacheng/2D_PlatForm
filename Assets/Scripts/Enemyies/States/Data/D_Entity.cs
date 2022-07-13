using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newEntityData", menuName ="Data/Entity Data/Base Data")]
public class D_Entity : ScriptableObject
{
    public float wallCheckDistance = 0.2f;
    public float ledgeCheckDistance = 0.4f;

    public float minAgroDistance = 3f; // 检测到玩家 产生仇恨的范围
    public float maxAgroDistance = 4f; // 玩家逃离, 取消仇恨状态的最大范围

    public LayerMask whatIsGround;
    public LayerMask whatIsPlayer;
}
