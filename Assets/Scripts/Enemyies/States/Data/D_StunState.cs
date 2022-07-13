using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "newStunStateData", menuName = "Data/State Data/Stun State Data")]
public class D_StunState : ScriptableObject
{
    [Header("眩晕时间")]
    public float stunTime = 3f;
    [Header("击退时间")]
    public float stunKnockbackTime = 0.2f;
    [Header("击退速度")]
    public float stunKnockbackSpeed = 20f;
    [Header("击退角度")]
    public Vector2 stunKnockbackAngle;
}
