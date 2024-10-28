using System.Collections.Generic;
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
    
    public CodeGraphAsset()
    {
        _nodes = new List<CodeGraphNode>();
        _connections = new List<CodeGraphConnection>();
    }
}
