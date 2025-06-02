using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[NodeInfo("Loop/ForEach")]
public class ForEachNode : CodeGraphNode
{
    private static int index = 0;

    const int LoopOutputIndex = 2;
    [Output] public PortType.FlowPort Loop;

    [Input] public object List;
    [Output] public object Current;

    public ForEachNode() : base("Foreach")
    {
    }

    public override string OnProcess(CodeGraphAsset graph)
    {
        IEnumerable<object> listObj = (List as IEnumerable).Cast<object>();
        if (index < listObj.Count())
        {
            Current = listObj.ElementAt(index++);
            return base.OnProcess(graph, LoopOutputIndex, ProcessType.Loop);
        }

        index = 0;
        return base.OnProcess(graph);
    }
}
