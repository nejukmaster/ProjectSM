using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;

    public Dictionary<string,Item> itemDictionary = new Dictionary<string,Item>();
    private void Awake()
    {
        instance = this;
    }
}
