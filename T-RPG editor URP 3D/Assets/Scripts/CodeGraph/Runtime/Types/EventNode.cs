using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[NodeInfo]
public class EventNode : CodeGraphNode
{
    [Input] public object Component;
    [Output] public PortType.FlowPort Event;

    public static int eventPortIndex = 3;

    public bool hasArgs = false;
    public bool hasReturn = false;

    private List<ParamInformation> parameters = null;
    public List<ParamInformation> Args
    {
        get
        {
            if (parameters != null) return parameters;
            List<ParamInformation> @params = new();

            foreach (var param in Assembly.GetExecutingAssembly().GetType(ClassName).GetEvent(EventName).EventHandlerType.GetMethod("Invoke").GetParameters())
            {
                ParamInformation paramInfo = new ParamInformation(null, param.Name, param.ParameterType.FullName);
                @params.Add(paramInfo);
            }
            parameters = @params;
            return @params;
        }
    }

    public string EventName;
    public string ClassName;

    public System.Type ClassType
    {
        get
        {
            return Assembly.GetExecutingAssembly().GetType(ClassName)
                ?? Assembly.GetAssembly(typeof(int)).GetType(ClassName);
        }
    }

    public EventNode() : base("")
    {

    }

    public EventNode(EventInfo eventInfo) : base(eventInfo.Name)
    {
        MethodInfo invokeMethod = eventInfo.EventHandlerType.GetMethod("Invoke");
        ParameterInfo[] @params = invokeMethod.GetParameters();

        EventName = eventInfo.Name;
        ClassName = eventInfo.ReflectedType.FullName;

        hasArgs = @params.Length > 0;
        hasReturn = invokeMethod.ReturnType != typeof(void);
    }

    public override string OnProcess(CodeGraphAsset graph)
    {
        return base.OnProcess(graph, OUTPUT, ProcessType.Event);
    }
}
