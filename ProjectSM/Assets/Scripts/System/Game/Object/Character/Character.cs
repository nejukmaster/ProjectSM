using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Status
{
    public string name;
    public int value;
    public int min;
    public int max;
    public Status(string name, int value, int min, int max)
    {
        this.name = name;
        this.value = value;
        this.min = min;
        this.max = max;
    }   
}
public class Character : MonoBehaviour
{
    [Header("Character Properties")]
    [Tooltip("캐릭터의 능력치 딕셔너리")]
    public SerializableDict<Status> statusDic;
    [Tooltip("이 캐릭터의 인벤토리 사이즈")]
    public int inventorySize;
    [Tooltip("이 캐릭터의 아이디")]
    public int id;

    public bool OnReady
    {
        get
        {
            return theObjectPool != null && statusDic.GetDic().Count > 0;
        }
        set
        {

        }
    }

    protected ObjectPool theObjectPool;
    protected Inventory inventory;

    [ExecuteAlways]
    private void Start()
    {
        statusDic = new SerializableDict<Status>();
        theObjectPool = ObjectPool.instance;
        FixedStart();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public virtual void FixedStart()
    {

    }

    public Inventory GetInventory()
    {
        return inventory;
    }

    public virtual void Despawn()
    {
        Stage.Instance.CharacterDespawnHook(this);
    }
}
