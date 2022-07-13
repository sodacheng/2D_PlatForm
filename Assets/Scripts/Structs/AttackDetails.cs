using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 造成伤害时传递的数据结构
/// </summary>
public struct AttackDetails
{
    public Vector2 position;
    public float damageAmout;

    public float stunDamageAmount;
}
