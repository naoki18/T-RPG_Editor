using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CodeGraph", menuName = "CodeGraph/NewGraph")]
public class CodeGraphAsset : ScriptableObject
{
    [SerializeReference]
    private List<CodeGraphNode> _nodes;
    public List<CodeGraphNode> Nodes => _nodes;
    public CodeGraphAsset()
    {
        _nodes = new List<CodeGraphNode>();
    }
}
