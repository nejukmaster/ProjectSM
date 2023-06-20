using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : Manager
{
    public static StageManager Instance;

    [System.Serializable]
    class StageInfo
    {
        public string name;
        public GameObject stagePrefab;
    }

    [SerializeField] StageInfo[] stageInfos;
    public override void Init()
    {
        Instance = this;
        string stageName = DataBus.Instance.data["stage"].String;
        for(int i = 0; i < stageInfos.Length; i++)
        {
            if (stageInfos[i].name == stageName)
            {
                Instantiate(stageInfos[i].stagePrefab);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
