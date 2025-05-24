using UnityEngine;

public class OrNode : CodeGraphNode
{
    [Input] public bool firstCondition;
    [Input] public bool secondCondition;
    [Output] public bool result;

    public OrNode() : base("Or")
    {
    }

    public override string OnProcess(CodeGraphAsset graph)
    {
        result = firstCondition || secondCondition;
        return base.OnProcess(graph);
    }
}
