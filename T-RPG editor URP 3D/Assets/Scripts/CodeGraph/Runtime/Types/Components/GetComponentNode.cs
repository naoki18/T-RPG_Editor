using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[NodeInfo("Get Component", "Entity/Get Component", true)]
public class GetComponentNode : CodeGraphNode
{
    [Input] public GameObject target;
    [ExposedProperty] public SystemTypeSerialisation type = new SystemTypeSerialisation();
    [Output] public object component;
    public override string OnProcess(CodeGraphAsset graph)
    {
        System.Type _type = System.Reflection.Assembly.GetExecutingAssembly().GetType(type.selectedType);
        Debug.Log(_type.Name);
        return base.OnProcess(graph);
    }
}
