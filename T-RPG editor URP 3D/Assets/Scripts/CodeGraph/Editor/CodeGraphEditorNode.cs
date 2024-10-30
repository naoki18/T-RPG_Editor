using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class CodeGraphEditorNode : Node
{
    private CodeGraphNode node;
    private Port _outputPort;
    private Port _inputPort;
    private List<Port> _ports;
    private SerializedObject _serializedObject;
    public CodeGraphNode Node => node;
    public List<Port> Ports => _ports;

    private SerializedProperty _serializedProperty;
    public CodeGraphEditorNode(CodeGraphNode _node, SerializedObject codeGraphObject)
    {
        this.AddToClassList("code-graph-node");
        this.node = _node;
        _ports = new List<Port>();
        Type type = _node.GetType();
        NodeInfoAttribute info = type.GetCustomAttribute<NodeInfoAttribute>();
        title = info.title;
        _serializedObject = codeGraphObject;

        string[] depth = info.menuItem.Split("/");
        foreach (string str in depth)
        {
            this.AddToClassList(str.ToLower().Replace(' ', '-'));
        }

        this.name = type.Name;
        if (info.hasFlowOutput)
        {
            CreateFlowOutput();
        }

        if (info.hasFlowInput)
        {
            CreateFlowInput();
        }

        foreach (FieldInfo variable in type.GetFields())
        {

            bool hasExposedProperty = variable.GetCustomAttribute<ExposedPropertyAttribute>() != null;
            bool hasInputProperty = variable.GetCustomAttribute<InputAttribute>() != null;
            bool hasOutputAttribute = variable.GetCustomAttribute<OutputAttribute>() != null;

            if (hasExposedProperty || hasInputProperty || hasOutputAttribute)
            {
                if(hasInputProperty) CreatePropertyInput(variable.Name, variable.FieldType);
                else if(hasOutputAttribute) CreatePropertyOutput(variable.Name, variable.FieldType);
                else DrawProperty(variable.Name);
                
            }
        }
        RefreshExpandedState();
    }


    private void FetchSerializedProperty()
    {
        SerializedProperty nodes = _serializedObject.FindProperty("_nodes");
        if(nodes.isArray)
        {
            int size = nodes.arraySize;
            for(int i = 0; i < size; i++)
            {
                var element = nodes.GetArrayElementAtIndex(i);
                var elementId = element.FindPropertyRelative("_guid");

                if(elementId.stringValue == node.id)
                {
                    _serializedProperty = element;
                }
            }
        }
    }
    private PropertyField DrawProperty(string name)
    {
        if(_serializedProperty == null)
        {
            FetchSerializedProperty();
        }

        SerializedProperty prop = _serializedProperty.FindPropertyRelative(name);

        PropertyField field = new PropertyField(prop);
        field.bindingPath = prop.propertyPath;
        extensionContainer.Add(field);
        return field;
    }

    private void CreatePropertyInput(string name, Type type)
    {
        Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, type);
        inputPort.portName = name;
        _ports.Add(inputPort);
        inputContainer.Add(inputPort);
    }

    private void CreatePropertyOutput(string name, Type type)
    {
        Port outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, type);
        outputPort.portName = name;

        _ports.Add(outputPort);
        outputContainer.Add(outputPort);
    }

    private void CreateFlowInput()
    {
        _inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(PortType.FlowPort));
        _inputPort.portName = "Input";
        
        _ports.Add(_inputPort);
        inputContainer.Add(_inputPort);
    }
    private void CreateFlowOutput()
    {
        _outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(PortType.FlowPort));
        _outputPort.portName = "Out";
        _ports.Add(_outputPort);
        outputContainer.Add(_outputPort);
    }
}
