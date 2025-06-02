using UnityEngine;

[NodeInfo("Game Objects/Find")]
public class FindGameObjectNode : CodeGraphNode
{
    [Output] public GameObject GameObject;
    [ExposedProperty] public SystemTypeSerialisation Type;
    public FindGameObjectNode() : base("Find")
    {
    }

    public override string OnProcess(CodeGraphAsset graph)
    {
        Object obj = Object.FindObjectOfType(Type.Type);
        GameObject = obj != null ? ((Component)obj).gameObject : null;

        return base.OnProcess(graph);
    }
}
