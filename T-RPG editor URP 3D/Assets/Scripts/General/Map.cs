using System;
using System.Collections.Generic;
using UnityEngine;

// Serialized Dictionnary
[Serializable]
public class Map<TKey, TValue>
{
    public void Init()
    {
        dict = new Dictionary<TKey, TValue>();
        List<TKey> keys = new List<TKey>();
        foreach (MapItem<TKey, TValue> pair in map)
        {
            if (keys.Contains(pair.key)) Debug.LogError(pair.key + " is used twice");
            dict.Add(pair.key, pair.value);
            keys.Add(pair.key);
        }
    }

    public TValue this[TKey key]
    {
        get => dict[key];
    }

    [SerializeField] MapItem<TKey, TValue>[] map;
    Dictionary<TKey, TValue> dict;
}

[Serializable]
public class MapItem<TKey, TValue>
{
    [SerializeField] public TKey key;
    [SerializeField] public TValue value;
}
