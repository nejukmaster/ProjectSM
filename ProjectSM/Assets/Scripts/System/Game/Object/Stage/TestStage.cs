using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestStage : Stage
{
    [System.Serializable]
    class NPCSpawnData
    {
        [Tooltip("������ ĳ���� Ÿ���Դϴ�. NPCŬ������ ���̵�� �����ϰ� �����־���մϴ�.")]
        public CharacterType type;
        [Tooltip("�ش� NPC�� ������ ObjectInfo�� �ε��� ����")]
        public int stagePoolIndex;
        [Tooltip("������ų ��������Ʈ ��ȣ")]
        public int spawnPointIndex;
    }
    [Tooltip("�� ������������ ����� ������ƮǮ�� �ε��� ����")]
    [SerializeField] NPCSpawnData[] datas;

    ObjectPool pool;


    public override void Init()
    {
        pool = ObjectPool.instance;
        for(int i = 0; i < datas.Length; i++)
        {
            pool.EnablePool(datas[i].stagePoolIndex, (int)datas[i].type);
            characterManager.SpawnEntity(datas[i].type,spawnPoints[datas[i].spawnPointIndex]);
        }
    }

    public override void Cleanup()
    {
        pool.DisablePools();
    }

    public override void CharacterDespawnHook(Character character)
    {

    }
}
