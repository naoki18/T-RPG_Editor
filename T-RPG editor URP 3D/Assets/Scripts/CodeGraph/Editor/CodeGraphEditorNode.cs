using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Experimental.GraphView;

public class CodeGraphEditorNode : Node
{
    private CodeGraphNode node;
    private Port _outputPort;
    private List<Port> _ports;
    public CodeGraphNode Node => node;
    public List<Port> Ports => _ports;
    public CodeGraphEditorNode(CodeGraphNode _node)
    {
        this.AddToClassList("code-graph-node");
        this.node = _node;
        _ports = new List<Port>();
        Type type = _node.GetType();
        NodeInfoAttribute info = type.GetCustomAttribute<NodeInfoAttribute>();
        title = info.title;

        string[] depth = info.menuItem.Split("/");
        foreach (string str in depth)
        {
            this.AddToClassList(str.ToLower().Replace(' ', '-'));
        }

        this.name = type.Name;

        if (info.hasFlowInput)
        {
            CreateFlowInput();
        }
        if (info.hasFlowOutput)
        {
            CreateFlowOutput();
        }
    }

    private void CreateFlowInput()
    {
        Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(PortType.FlowPort));
        inputPort.portName = "Input";
        
        _ports.Add(inputPort);
        outputContainer.Add(inputPort);
    }
    private void CreateFlowOutput()
    {
        _outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(PortType.FlowPort));
        _outputPort.portName = "Out";
        _ports.Add(_outputPort);
        outputContainer.Add(_outputPort);
    }
}
