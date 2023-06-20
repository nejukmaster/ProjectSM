using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StageEditor))]
public abstract class Stage : MonoBehaviour
{
    public static Stage Instance;

    protected CharacterManager characterManager;

    [SerializeField] public Vector3[] spawnPoints;

    public abstract void Init();

    public abstract void Cleanup();

    public virtual void CharacterDespawnHook(Character character){ }

    public virtual void CharacterSpawnHook(Character character) { }

    public virtual void CharacterDeathHook(Character character) { }

    public virtual void CharacterHitOtherHook(Character attacker, Character victim)
    {

    }

    private void Start()
    {
        Instance = this;
        characterManager = CharacterManager.instance;
        Init();
    }
}
