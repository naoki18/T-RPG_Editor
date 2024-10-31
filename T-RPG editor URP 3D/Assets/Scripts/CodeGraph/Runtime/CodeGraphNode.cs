using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

[Serializable]
public class CodeGraphNode
{
    [Serializable]
    public struct LinkedValue
    {
        public string inputValue;
        public string outputValue;
        public string inputNodeId;
        public LinkedValue(string _inputValue, string _outputValue, string _inputNodeId)
        {
            inputValue = _inputValue;
            outputValue = _outputValue;
            inputNodeId = _inputNodeId;
        }
    }

    [SerializeField] private string _guid;
    [SerializeField] private Rect _position;
    public string typeName;
    public string id => _guid;
    public Rect position => _position;
    public List<LinkedValue> linkedValues;
    public CodeGraphNode()
    {
        linkedValues = new List<LinkedValue>();
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

    public virtual string OnProcess(CodeGraphAsset graph)
    {
        CodeGraphNode node = graph.GetNextNode(_guid, 0);
        if (linkedValues.Count > 0)
        {
            foreach (LinkedValue linkedValue in linkedValues)
            {
                CodeGraphNode inputNode = graph.GetNode(linkedValue.inputNodeId);
                object _outputValue = this.GetType().GetField(linkedValue.outputValue).GetValue(this);
                if(_outputValue != null)
                    inputNode.GetType().GetField(linkedValue.inputValue).SetValue(inputNode, _outputValue);
            }
        }
        if (node != null)
        {
            return node.id;
        }
        return string.Empty;
    }
}
