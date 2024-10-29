using System;
using UnityEngine;


[NodeInfo("Debug Log", "Debug/Log")]
public class DebugLogNode : CodeGraphNode
{
    
    [ExposedProperty()] public string message;
    public override string OnProcess(CodeGraphAsset graph)
    {
        Debug.Log(message);
        return base.OnProcess(graph);
    }
}
