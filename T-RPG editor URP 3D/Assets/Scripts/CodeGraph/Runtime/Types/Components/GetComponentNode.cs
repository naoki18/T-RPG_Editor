using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[NodeInfo("Get Component", "Entity/Get Component", true)]
public class GetComponentNode : CodeGraphNode
{
    [Input] public GameObject target;
    //[ExposedProperty] public SerializableSystemType componentType = new SerializableSystemType(typeof(Weapon));
    [ExposedProperty] public SystemTypeSerialisation test = new SystemTypeSerialisation();
    [Output] public object component;
    public override string OnProcess(CodeGraphAsset graph)
    {
        //component = target.GetComponent(componentType.SystemType);
        return base.OnProcess(graph);
    }
}
