using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[CreateAssetMenu(fileName = "CodeGraph", menuName = "CodeGraph/NewGraph")]
public class CodeGraphAsset : ScriptableObject
{
    [SerializeReference]
    private List<CodeGraphNode> _nodes;
    [SerializeField]
    List<CodeGraphConnection> _connections;
    public List<CodeGraphNode> Nodes => _nodes;
    public List<CodeGraphConnection> Connections => _connections;

    private Dictionary<string, CodeGraphNode> _nodeDict;
    public Dictionary<string, CodeGraphNode> NodeDict => _nodeDict;
    public CodeGraphAsset()
    {
        _nodes = new List<CodeGraphNode>();
        _connections = new List<CodeGraphConnection>();
    }

    public void Init()
    {
        _nodeDict = new Dictionary<string, CodeGraphNode>();
        foreach (var node in _nodes)
        {
            _nodeDict.Add(node.id, node);
        }
    }
    public CodeGraphNode GetStartNode()
    {
        StartNode node = Nodes.OfType<StartNode>().FirstOrDefault();
        if (node == null)
        {
            Debug.LogError("Can't find start node in ");
            return null;
        }
        return node;
    }

    public CodeGraphNode GetNode(string nodeId)
    {
        return _nodeDict.TryGetValue(nodeId, out var node) ? node : null;
    }

    public CodeGraphNode GetNextNode(string outputNodeId, int portIndex)
    {
        foreach (var connection in _connections)
        {
            if (connection.outputPort.nodeId == outputNodeId && connection.outputPort.portIndex == portIndex)
            {
                string id = connection.inputPort.nodeId;
                return GetNode(id); 
            }
        }
        return null;
    }
}