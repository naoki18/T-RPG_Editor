using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[NodeInfo("Loop/For")]
public class ForNode : CodeGraphNode
{
    private static int count = 0;

    const int LoopOutputIndex = 2;
    [Output] public PortType.FlowPort Loop;

    [Input, ExposedProperty] public int beginIndex = 0;
    [Input, ExposedProperty] public int endIndex = 0;
    [Input, ExposedProperty] public int step = 0;

    public ForNode() : base("For")
    {
    }

    public override string OnProcess(CodeGraphAsset graph)
    {
        if(step == 0)
        {
            throw new System.Exception("Step can't be equal to 0 to avoid looping infinitely");
        }
        if ((step > 0 && count < endIndex) || (step < 0 && count > endIndex))
        {
            count += step;
            return base.OnProcess(graph, LoopOutputIndex, ProcessType.Loop);
        }

        count = 0;
        return base.OnProcess(graph);
    }
}
