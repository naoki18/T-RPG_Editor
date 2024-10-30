using UnityEngine;

[NodeInfo("Delta time", "Time/Delta Time", false, true, true)]
public class DeltaTimeNode : CodeGraphNode
{
    public float DeltaTime;
    public override string OnProcess(CodeGraphAsset graph)
    {
        DeltaTime = Time.deltaTime;
        return base.OnProcess(graph);
    }
}
