using UnityEngine;

[NodeInfo("Transform/Move")]
public class MoveNode : CodeGraphNode
{
    [ExposedProperty, Input] public Vector3 direction;
    [ExposedProperty] public float speed;

    public MoveNode() : base("Move")
    {
    }

    public override string OnProcess(CodeGraphAsset graph)
    {
        direction = direction.normalized;
        graph.gameObject.transform.position += direction * speed;
        return base.OnProcess(graph);
    }
}
