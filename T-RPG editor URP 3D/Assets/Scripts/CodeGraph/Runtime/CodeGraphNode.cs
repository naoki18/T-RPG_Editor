using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CodeGraphNode
{
    [SerializeField] private string _guid;
    [SerializeField] private Rect _position;

    public string typeName;
    public string id => _guid;
    public Rect position => _position;

    public CodeGraphNode()
    {
        NewGUID();
    }

    private void NewGUID()
    {
        _guid = Guid.NewGuid().ToString();
    }

    public void SetPosition(Rect position)
    {
        _position = position; 
    }
}
