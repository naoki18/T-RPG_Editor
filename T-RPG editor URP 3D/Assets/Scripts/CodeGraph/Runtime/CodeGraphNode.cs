using System;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

[Serializable]
public class CodeGraphNode
{
    [SerializeField] private string _guid;
    [SerializeField] private Rect _position;
    public string typeName;
    public string id => _guid;
    public Rect position => _position;
    public string outputLinkedValue;
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

    public virtual string OnProcess(CodeGraphAsset graph, object outputValue)
    {
        CodeGraphNode node = graph.GetNextNode(_guid, 0);
        if(outputValue != null)
        {
            node.GetType().GetField(outputLinkedValue).SetValue(node, outputValue);
        }
        if (node != null)
        {
            return node.id;
        }
        return string.Empty;
    }
}
