using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 冲刺
/// 如果能够冲刺, 会将角色的速度设置到较高值并保持该速度 持续时间或直到被阻挡
/// 残影通过实例化与角色相同的精灵到角色游戏对象的位置上来创建, 然后通过随时间来减少精灵的Alpha
/// </summary>
public class PlayerAfterImageSprite : MonoBehaviour
{
    [SerializeField]
    private float activeTime = 0.1f; // 残影激活(跟踪)的时间
    private float timeActivated; // 已经跟踪的时间
    private float alpha; // 记录当前SpriteRenderer的Color中的Alpha值
    [SerializeField]
    private float alphaSet = 0.8f; // 创建残影时Alpha的初始值
    [SerializeField]
    private float alphaDecay; // alpha衰减
    //private float alphaMultiplier = 0.85f; // 残影的变化速度

    private Transform player;

    private SpriteRenderer SR; // 对自己SpriteRenderer的引用
    private SpriteRenderer playerSR; // 对Player的SpriteRenderder的引用

    private Color color; // 随时间更改Alpha,表现冲刺的残影

    // 每次启用游戏对象时被调用
    private void OnEnable()
    {
        SR = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerSR = player.GetComponent<SpriteRenderer>();

        alpha = alphaSet;
        SR.sprite = playerSR.sprite;
        transform.position = player.position;
        transform.rotation = player.rotation;

        timeActivated = Time.time;
    }

    private void Update()
    {
        alpha -= alphaDecay * Time.deltaTime; // 随时间淡化残影
        color = new Color(1, 1, 1, alpha);
        SR.color = color;

        if(Time.time >= (timeActivated + activeTime))
        {
            // 放回对象池
            PlayerAfterImagePool.Instance.AddToPool(gameObject);
        }
    }
}
