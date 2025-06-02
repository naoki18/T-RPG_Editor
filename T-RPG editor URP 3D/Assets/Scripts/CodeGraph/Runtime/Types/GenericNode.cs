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
    [Input] public object Component;
    [Output] public object Returned;


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

    public System.Type ReturnType { get
        {
            return Assembly.GetExecutingAssembly().GetType(ReturnTypeName)
                ?? Assembly.GetAssembly(typeof(int)).GetType(ReturnTypeName);
        }
    }

    public System.Type ClassType
    {
        get
        {
            return Assembly.GetExecutingAssembly().GetType(ClassName)
                ?? Assembly.GetAssembly(typeof(int)).GetType(ClassName);
        }
    }

    public List<string> argsTypeName { get; } = new();
    public GenericNode() : base("")
    {

    }

    public GenericNode(MethodInfo method) : base(method.Name)
    {
        var @params = method.GetParameters();

        MethodName = method.Name;
        ClassName = method.ReflectedType.FullName;

        hasArgs = @params.Length > 0;
        ReturnTypeName = method.ReturnType.FullName;
        hasReturn = method.ReturnType != typeof(void);
    }

    public override string OnProcess(CodeGraphAsset graph)
    {
        try
        {
            object[] args = Args.Select(x => x.Value).ToArray();
            Returned = Assembly.GetExecutingAssembly().GetType(ClassName).GetMethod(MethodName).Invoke(Component, args);
            
        }
        catch (Exception) 
        {
            // Do nothing
        }

        return base.OnProcess(graph);

    }
}