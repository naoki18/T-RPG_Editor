[NodeInfo("Add", "Maths/Add", true, true)]
public class AddNode : CodeGraphNode
{
    [ExposedProperty] public int a;
    [ExposedProperty] public int b;
    [Output] public int result;
    public override string OnProcess(CodeGraphAsset graph, object outputValue)
    {
        result = a + b;
        return base.OnProcess(graph, result);
    }
}
