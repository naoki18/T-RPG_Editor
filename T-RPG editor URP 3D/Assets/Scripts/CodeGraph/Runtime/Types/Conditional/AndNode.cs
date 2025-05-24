using UnityEngine;

[NodeInfo("Conditional/And")]
public class AndNode : CodeGraphNode
{
    [Input] public bool firstCondition;
    [Input] public bool secondCondition;
    [Output] public bool result;

    public AndNode() : base("And")
    {
    }

    public override string OnProcess(CodeGraphAsset graph)
    {
        result = firstCondition && secondCondition;
        return base.OnProcess(graph);
    }
}
