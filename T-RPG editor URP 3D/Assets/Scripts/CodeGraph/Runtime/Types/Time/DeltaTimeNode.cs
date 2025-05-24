using UnityEngine;

[NodeInfo("Time/Delta Time", false, true, true)]
public class DeltaTimeNode : CodeGraphNode
{
    public float DeltaTime;

    public DeltaTimeNode() : base("Delta Time")
    {
    }

    public override string OnProcess(CodeGraphAsset graph)
    {
        DeltaTime = Time.deltaTime;
        return base.OnProcess(graph);
    }
}
