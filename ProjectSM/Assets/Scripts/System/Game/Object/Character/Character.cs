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
    [Tooltip("ĳ������ �ɷ�ġ ��ųʸ�")]
    public SerializableDict<Status> statusDic;
    [Tooltip("�� ĳ������ �κ��丮 ������")]
    public int inventorySize;
    [Tooltip("�� ĳ������ ���̵�")]
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
