using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;

public class CodeGraphEditorNode : Node
{
    private CodeGraphNode node;
    private Port _outputPort;
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
            if (variable.GetCustomAttribute<ExposedPropertyAttribute>() != null)
            {
                PropertyField field = DrawProperty(variable.Name);
                //field.RegisterValueChangeCallback(OnFieldChange);
            }
        }
        RefreshExpandedState();
    }

    private void OnFieldChange(SerializedPropertyChangeEvent evt)
    {
        throw new NotImplementedException();
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
