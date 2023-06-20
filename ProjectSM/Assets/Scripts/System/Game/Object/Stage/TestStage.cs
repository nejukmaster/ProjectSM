using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestStage : Stage
{
    [System.Serializable]
    class NPCSpawnData
    {
        [Tooltip("생성할 캐릭터 타입입니다. NPC클래스의 아이디와 동일하게 맞춰주어야합니다.")]
        public CharacterType type;
        [Tooltip("해당 NPC를 생성할 ObjectInfo의 인덱스 정보")]
        public int stagePoolIndex;
        [Tooltip("스폰시킬 스폰포인트 번호")]
        public int spawnPointIndex;
    }
    [Tooltip("이 스테이지에서 사용할 오브젝트풀의 인덱스 정보")]
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
