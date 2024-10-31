using UnityEngine;

[NodeInfo("Move", "Transform/Move")]
public class MoveNode : CodeGraphNode
{
    [ExposedProperty] public Vector3 vector;
    [ExposedProperty] public float speed;
    public override string OnProcess(CodeGraphAsset graph)
    {
        graph.gameObject.transform.position += vector;
        return base.OnProcess(graph);
    }
}
