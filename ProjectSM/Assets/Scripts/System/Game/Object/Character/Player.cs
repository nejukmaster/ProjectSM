using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public static Player Instance;
    [Header("Player Properties")]
    public float gauge = 0;
    public override void FixedStart()
    {
        List<Dictionary<string, object>> statusData = CSVReader.Read("DataSheet/Status");
        for(int i = 0; i < statusData.Count; i++)
        {
            if (!(statusData[i]["target"].ToString() == "1"))
            {
                Status _st = new Status(statusData[i]["name"].ToString(), 0, System.Int32.Parse(statusData[i]["min"].ToString()), System.Int32.Parse(statusData[i]["max"].ToString()));
                statusDic.data.Add(new SerializeData<Status>(statusData[i]["code"].ToString(), _st));
            }
        }
        Instance = this;
    }
}
