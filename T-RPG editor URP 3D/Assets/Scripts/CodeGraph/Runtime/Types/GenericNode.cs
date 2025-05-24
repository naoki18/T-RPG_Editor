using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;

public class ParamInformation
{
#nullable enable
    public ParamInformation(object? value, string name, string typeName)
    {
        Name = name;
        Value = value;
        _typeName = typeName;
    }

    private string _typeName;

    public string Name;
    public object? Value;
#nullable disable
    public Type Type
    {
        get
        {
            Type type = Type.GetType(_typeName)
          ?? Assembly.GetExecutingAssembly().GetType(_typeName)
          ?? typeof(int).Assembly.GetType(_typeName);
            return type;
        }
    }
}

[NodeInfo]
public class GenericNode : CodeGraphNode
{
    [Input] public object component;
    [Output] public object returned;


    public bool hasArgs = false;
    public bool hasReturn = false;

    private List<ParamInformation> parameters = null;
    public List<ParamInformation> Args
    {
        get
        {
            if (parameters != null) return parameters;
            List<ParamInformation> @params = new();

            foreach (var param in Assembly.GetExecutingAssembly().GetType(ClassName).GetMethod(MethodName).GetParameters())
            {
                ParamInformation paramInfo = new ParamInformation(null, param.Name, param.ParameterType.FullName);
                @params.Add(paramInfo);
            }
            parameters = @params;
            return @params;
        }
    }

    public string MethodName;
    public string ClassName;
    public string ReturnTypeName;

    public System.Type ReturnType { get => Assembly.GetExecutingAssembly().GetType(ReturnTypeName); }
    
    public List<string> argsTypeName { get; } = new();
    public GenericNode() : base("")
    {

    }

    public GenericNode(MethodInfo method) : base(method.Name)
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
        try
        {
            object[] args = Args.Select(x => x.Value).ToArray();
            returned = Assembly.GetExecutingAssembly().GetType(ClassName).GetMethod(MethodName).Invoke(component, args);
            return base.OnProcess(graph);
        }
        catch (Exception)
        {
            return string.Empty;
        }

    }
}