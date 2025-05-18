[NodeInfo("Short value", "Litteral/Short", true, true, true)]
public class LitteralShort : CodeGraphNode
{
    [ExposedProperty] public short value;
    [Output] public short outputValue;
    public override string OnProcess(CodeGraphAsset graph)
    {
        outputValue = value;
        return base.OnProcess(graph);
    }
}
