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
    public SerializableDict<Status> statusDic;
    public int inventorySize;

    Inventory inventory;

    [ExecuteAlways]
    private void Start()
    {
        statusDic = new SerializableDict<Status>();
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
}
