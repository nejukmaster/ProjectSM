using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializeData<T>
{
    public string key;
    public T value;

    public SerializeData(string key, T value)
    {
        this.key = key;
        this.value = value;
    }
}
[System.Serializable]
public class SerializableDict<T>
{
    public List<SerializeData<T>> data = new List<SerializeData<T>>();
    public Dictionary<string, T> GetDic()
    {
        Dictionary<string, T> dict = new Dictionary<string, T>();
        for(int i = 0; i < data.Count; i++)
        {
            dict[data[i].key] = data[i].value;
        }
        return dict;
    }
}
