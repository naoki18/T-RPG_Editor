using UnityEngine;

[NodeInfo("Start node", "Process/Start", false, true)]
public class StartNode : CodeGraphNode
{
    public override string OnProcess(CodeGraphAsset graph, object outputValue)
    {
        Debug.Log("Start node");
        return base.OnProcess(graph, null);
    }
}
