using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterType
{
    Player = 0,
    Dummy = 1
}
public class CharacterManager : Manager
{
    public static CharacterManager instance;

    ObjectPool theObjectPool;

    void SpawnEntity(int id, Vector3 pos)
    {
        GameObject go = theObjectPool.QueueList[id].Dequeue();
        go.transform.position = pos;
        go.SetActive(true);
        Stage.Instance.CharacterSpawnHook(go.GetComponent<Character>());
    }

    public void SpawnEntity(CharacterType type, Vector3 pos)
    {
        switch(type)
        {
            case CharacterType.Player:
                SpawnEntity(0,pos);
                break;
            case CharacterType.Dummy:
                SpawnEntity(1, pos); 
                break;
        }
    }

    public override void Init()
    {
        instance = this;
        theObjectPool = ObjectPool.instance;
    }
}
