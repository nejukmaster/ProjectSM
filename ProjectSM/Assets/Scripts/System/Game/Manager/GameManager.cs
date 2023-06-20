using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Manager
{
    public static GameManager instance;

    public bool onGame;

    [SerializeField] Manager[] managers;
    public override void Init()
    {
        instance = this;
        for (int i = 0; i < managers.Length; i++)
        {
            managers[i].Init();
        }
        onGame = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }
}
