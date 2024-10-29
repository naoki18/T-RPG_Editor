using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[NodeInfo("ToString", "Cast/ToString")]
public class ToStringNode : CodeGraphNode
{
    [Input] public object toCast;
    [Output] public string fromCast;
    public override string OnProcess(CodeGraphAsset graph, object outputValue)
    {
        fromCast = toCast.ToString();
        return base.OnProcess(graph, fromCast);
    }
}
