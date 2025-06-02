using UnityEngine;

[NodeInfo("Cast/ToString")]
public class ToStringNode : CodeGraphNode
{
    [Input] public object toCast;
    [Output] public string fromCast;

    public ToStringNode() : base("To String")
    {
    }

    public override string OnProcess(CodeGraphAsset graph)
    {
        fromCast = toCast == null ? "" : toCast.ToString();
        return base.OnProcess(graph);
    }
}
