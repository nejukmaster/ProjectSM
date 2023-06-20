using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : Manager
{
    public static ItemManager instance;

    public Dictionary<string,Item> itemDictionary = new Dictionary<string,Item>();

    public override void Init()
    {
        instance = this;
    }

}
