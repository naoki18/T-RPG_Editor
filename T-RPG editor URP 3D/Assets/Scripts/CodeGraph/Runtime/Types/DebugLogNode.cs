using UnityEngine;

[NodeInfo("Debug Log", "Debug/Log")]
public class DebugLogNode : CodeGraphNode
{
    [Input] public string message;
    public override string OnProcess(CodeGraphAsset graph, object outputValue)
    {
        Debug.Log(message);
        return base.OnProcess(graph, null);
    }
}
