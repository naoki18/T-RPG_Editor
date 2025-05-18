using UnityEngine;

[NodeInfo("Get Component", "Entity/Get Component", true)]
public class GetComponentNode : CodeGraphNode
{
    [Input] public GameObject target;
    [Output] public object component;
    [ExposedProperty] public SystemTypeSerialisation type = new();
    public override string OnProcess(CodeGraphAsset graph)
    {
        System.Type _type = type.Type;
        component = target.GetComponent(_type);
        return base.OnProcess(graph);
    }
}
