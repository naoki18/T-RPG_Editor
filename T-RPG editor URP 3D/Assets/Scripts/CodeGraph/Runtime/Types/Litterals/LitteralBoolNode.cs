[NodeInfo("Litteral/Bool")]
public class LitteralBoolNode : CodeGraphNode
{
    [ExposedProperty] public bool value;
    [Output] public bool outputValue;

    public LitteralBoolNode() : base("Bool Value")
    {
    }

    public override string OnProcess(CodeGraphAsset graph)
    {
        outputValue = value;
        return base.OnProcess(graph);
    }
}
