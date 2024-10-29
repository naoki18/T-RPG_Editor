using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[NodeInfo("Update", "Process/Update", false, true)]
public class UpdateNode : CodeGraphNode
{
    public override string OnProcess(CodeGraphAsset graph, object outputValue)
    {
        return base.OnProcess(graph, null);
    }
}
