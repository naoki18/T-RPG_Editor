using Unity.VisualScripting;
using UnityEngine;

[NodeInfo("Debug Log", "Debug/Log")]
public class DebugLogNode : CodeGraphNode
{
    [Input] public string message;
    [Input] public Color color; 
    public override string OnProcess(CodeGraphAsset graph)
    {
        if (color == new Color(0,0,0,0)) color = Color.white;
        Debug.Log("<color=#" + color.ToHexString() + ">" + message + "</color>");
        return base.OnProcess(graph);
    }
}
