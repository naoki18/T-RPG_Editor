[NodeInfo("Litteral/Int", true, true, true)]
public class LitteralIntNode : CodeGraphNode
{
    [ExposedProperty] public int value;
    [Output] public int outputValue;

    public LitteralIntNode() : base("Int 32")
    {
    }

    public override string OnProcess(CodeGraphAsset graph)
    {
        outputValue = value;
        return base.OnProcess(graph);
    }
}
