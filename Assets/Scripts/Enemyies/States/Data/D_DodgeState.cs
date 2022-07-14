using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "newDodgeStateData", menuName = "Data/State Data/Dodge State")]
public class D_DodgeState : ScriptableObject
{
    [Header("闪避速度")]
    public float dodgeSpeed = 10f;
    [Header("闪避状态时长")]
    public float dodgeTime = 0.2f;
    [Header("闪避冷却")]
    public float dodgeCooldown = 2f;
    [Header("闪退角度")]
    public Vector2 dodgeAngle;
}
