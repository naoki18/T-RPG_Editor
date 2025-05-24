using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[NodeInfo("Conditional/If", false, true, false)]
public class IfNode : CodeGraphNode
{
    const int trueIndex = 2;
    const int falseIndex = 3;

    [Input] public bool Condition;
    [Output] public PortType.FlowPort True;
    [Output] public PortType.FlowPort False;

    public IfNode() : base("If")
    {

    }

    public override string OnProcess(CodeGraphAsset graph)
    {
        int output = Condition ? trueIndex : falseIndex;
        return base.OnProcess(graph, output);
    }
}
