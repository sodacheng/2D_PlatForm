using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 简化的对象池, 用于存取残影对象
/// </summary>
public class PlayerAfterImagePool : MonoBehaviour
{
    [SerializeField]
    private GameObject afterImagePrefab;

    private Queue<GameObject> availableObjects = new Queue<GameObject>(); // 存储当前不活动的对象


    #region 单例
    public static PlayerAfterImagePool Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
        GrowPool();
    }
    #endregion

    /// <summary>
    /// 创建10个残影对象,存进对象池
    /// </summary>
    private void GrowPool()
    {
        for(int i = 0; i < 10; i++)
        {
            var instanceToAdd = Instantiate(afterImagePrefab);
            instanceToAdd.transform.SetParent(transform);
            AddToPool(instanceToAdd);
        }
    }

    public void AddToPool(GameObject instance)
    {
        instance.SetActive(false);
        availableObjects.Enqueue(instance);
    }

    public GameObject GetFromPool()
    {
        if(availableObjects.Count == 0)
        {
            GrowPool();
        }

        var instance = availableObjects.Dequeue();
        instance.SetActive(true);
        return instance;
    }
}
