using UnityEngine;

[NodeInfo("Conditional/Not")]
public class NotNode : CodeGraphNode
{
    [Input] public bool condition;
    [Output] public bool result;

    public NotNode() : base("Not")
    {
    }

    public override string OnProcess(CodeGraphAsset graph)
    {
        result = !condition;
        return base.OnProcess(graph);
    }
}
