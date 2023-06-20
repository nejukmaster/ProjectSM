using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBus : MonoBehaviour
{
    public struct Dataset
    {
        public float Float;
        public int Int;
        public double Double;
        public string String;
    }
    public static DataBus Instance;

    public Dictionary<string, Dataset> data = new Dictionary<string, Dataset>();

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static void Add(string key, float value)
    {
        Dataset ds = new Dataset();
        ds.Float = value;
        if (Instance.data.ContainsKey(key))
        {
            Instance.data.Remove(key);
            Instance.data[key] = ds;
        }
        else
            Instance.data.Add(key, ds);
    }public static void Add(string key, string value)
    {
        Dataset ds = new Dataset();
        ds.String = value;
        if (Instance.data.ContainsKey(key))
        {
            Instance.data.Remove(key);
            Instance.data[key] = ds;
        }
        else
            Instance.data.Add(key, ds);
    }public static void Add(string key, int value)
    {
        Dataset ds = new Dataset();
        ds.Int = value;
        if (Instance.data.ContainsKey(key))
        {
            Instance.data.Remove(key);
            Instance.data[key] = ds;
        }
        else
            Instance.data.Add(key, ds);
    }public static void Add(string key, double value)
    {
        Dataset ds = new Dataset();
        ds.Double = value;
        if (Instance.data.ContainsKey(key))
        {
            Instance.data.Remove(key);
            Instance.data[key] = ds;
        }
        else
            Instance.data.Add(key, ds);
    }
}
