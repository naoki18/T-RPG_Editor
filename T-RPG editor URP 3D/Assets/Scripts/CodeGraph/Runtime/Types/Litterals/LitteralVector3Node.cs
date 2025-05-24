using UnityEngine;


[NodeInfo("Litteral/Vector3", true, true, true)]
public class LitteralVector3Node : CodeGraphNode
{
    [ExposedProperty] public Vector3 value;
    [Output] public Vector3 outputValue;

    public LitteralVector3Node() : base("Vector3 value")
    {

    }

    public override string OnProcess(CodeGraphAsset graph)
    {
        outputValue = value;
        return base.OnProcess(graph);
    }

}
