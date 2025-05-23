using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[NodeInfo("Process/Update", false, false, true)]
public class UpdateNode : CodeGraphNode
{
    public UpdateNode() : base("Update")
    {
    }

    public override string OnProcess(CodeGraphAsset graph)
    {
        return base.OnProcess(graph);
    }
}
