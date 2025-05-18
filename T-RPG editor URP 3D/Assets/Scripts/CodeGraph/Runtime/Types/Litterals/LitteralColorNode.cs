using UnityEngine;

[NodeInfo("Litteral/Color", true, true, true)]
public class LitteralColorNode : CodeGraphNode
{
    [ExposedProperty] public Color value;
    [Output] public Color outputValue;

    public LitteralColorNode() : base("Color")
    {
    }

    public override string OnProcess(CodeGraphAsset graph)
    {
        outputValue = value;
        return base.OnProcess(graph);
    }
}
