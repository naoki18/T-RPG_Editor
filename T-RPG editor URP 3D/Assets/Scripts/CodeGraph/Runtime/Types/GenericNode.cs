using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;

[NodeInfo("Generic node")]
public class GenericNode : CodeGraphNode
{
    [Input] public object component;
    [Input] public List<object> args = new();
    [Output] public object returned = null;

    public bool hasArgs = false;
    public bool hasReturn = false;

    public string MethodName { get; set; }
    public string ClassName { get; set; }

    public System.Type ReturnType { get; }
    public GenericNode()
    {

    }

    public GenericNode(MethodInfo method)
    {
        var @params = method.GetParameters();

        MethodName = method.Name;
        ClassName = method.ReflectedType.Name;
        foreach(var param in @params)
        {
            args.Add(param);
        }

        hasArgs = args.Count > 0;
        ReturnType = method.ReturnType;
        hasReturn = method.ReturnType != typeof(void);

        this.GetType().GetAttribute<NodeInfoAttribute>().title = MethodName;
    }

    public override string OnProcess(CodeGraphAsset graph)
    {
        returned = Assembly.GetExecutingAssembly().GetType(ClassName).GetMethod(MethodName).Invoke(component, args.ToArray());
        return base.OnProcess(graph);
    }
}
