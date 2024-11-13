using UnityEngine;


[NodeInfo("Vector3 value", "Litteral/Vector3", true, true, true)]
public class LitteralVector3Node : CodeGraphNode
{
    [ExposedProperty] public Vector3 value;
    [Output] public Vector3 outputValue;

    public override string OnProcess(CodeGraphAsset graph)
    {
        outputValue = value;
        return base.OnProcess(graph);
    }

}
