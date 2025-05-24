using UnityEngine;

[NodeInfo("Process/Start", false, false, true)]
public class StartNode : CodeGraphNode
{
    public StartNode() : base("Start node")
    {

    }
    public override string OnProcess(CodeGraphAsset graph)
    {
        Debug.Log("Start node");
        return base.OnProcess(graph);
    }
}
