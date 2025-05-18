using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[NodeInfo("Get self", "Self/Get self")]
public class ThisNode : CodeGraphNode
{
    [Output] public GameObject self;
    public override string OnProcess(CodeGraphAsset graph)
    {
        self = graph.gameObject;
        return base.OnProcess(graph);
    }
}
