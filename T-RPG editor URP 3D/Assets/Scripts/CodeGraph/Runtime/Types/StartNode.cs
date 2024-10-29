using UnityEngine;

[NodeInfo("Start node", "Process/Start", false, true)]
public class StartNode : CodeGraphNode
{
    public override string OnProcess(CodeGraphAsset graph)
    {
        Debug.Log("Start node");
        return base.OnProcess(graph);
    }
}
