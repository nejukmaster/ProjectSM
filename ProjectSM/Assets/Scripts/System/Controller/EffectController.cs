using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

[System.Serializable]
public class EffectDic
{
    public string key;
    public ParticleSystem value;
}
public class EffectController : MonoBehaviour
{
    [SerializeField] List<EffectDic> effectDictionary = new List<EffectDic> ();
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i <effectDictionary.Count; i++)
        {
            effectDictionary[i].value.Stop();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayEffect(string key)
    {
        for(int i = 0; i < effectDictionary.Count; i++)
        {
            if (effectDictionary[i].key == key)
            {
                effectDictionary[i].value.Play();
            }
        }
    }

    public void PlayEffect(string key, Vector3 position)
    {
        for (int i = 0; i < effectDictionary.Count; i++)
        {
            if (effectDictionary[i].key == key)
            {
                effectDictionary[i].value.transform.position = position;
                effectDictionary[i].value.Play();
            }
        }
    }

    public void StopEffect(string key)
    {
        for (int i = 0; i < effectDictionary.Count; i++)
        {
            if (effectDictionary[i].key == key)
            {
                effectDictionary[i].value.Stop();
            }
        }
    }
}
