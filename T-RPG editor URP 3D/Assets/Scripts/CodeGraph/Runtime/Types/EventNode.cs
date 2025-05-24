using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[NodeInfo]
public class EventNode : CodeGraphNode
{
    public static int eventPortIndex = 3;

    public bool hasArgs = false;
    public bool hasReturn = false;
    public string MethodName;
    public string ClassName;
    public string ReturnTypeName;

    [Input] public object component;
    [Output] public PortType.FlowPort Event;

    public EventNode() : base("")
    {

    }

    public EventNode(MethodInfo method) : base(method.Name.Replace("add_", ""))
    {
        var @params = method.GetParameters();

        MethodName = method.Name;
        ClassName = method.ReflectedType.Name;

        hasArgs = @params.Length > 0;
        ReturnTypeName = method.ReturnType.Name;
        hasReturn = method.ReturnType != typeof(void);
    }

    public override string OnProcess(CodeGraphAsset graph)
    {
        return base.OnProcess(graph);
    }
}
