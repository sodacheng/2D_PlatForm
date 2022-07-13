using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "newDeadData", menuName = "Data/State Data/Dead State Data")]
public class D_DeadState : ScriptableObject
{
    [Header("死亡创建的特效")]
    public GameObject deathChunkParticle;
    public GameObject deathBloodParticle;
}
