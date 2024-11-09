using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
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
    List<VisualElement> rows;
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
        rows = new List<VisualElement>();
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

        int index = 0;
        foreach (FieldInfo variable in type.GetFields())
        {
            bool hasExposedProperty = variable.GetCustomAttribute<ExposedPropertyAttribute>() != null;
            bool hasInputProperty = variable.GetCustomAttribute<InputAttribute>() != null;
            bool hasOutputAttribute = variable.GetCustomAttribute<OutputAttribute>() != null;

            if (hasExposedProperty || hasInputProperty || hasOutputAttribute)
            {
                VisualElement newRow = new VisualElement();
                newRow.style.flexDirection = FlexDirection.Row;
                rows.Add(newRow);
                if (hasInputProperty) CreatePropertyInput(variable.Name, variable.FieldType, index);
                else if (hasOutputAttribute) CreatePropertyOutput(variable.Name, variable.FieldType);
                if (hasExposedProperty) DrawProperty(variable.Name, index);
                index++;

            }
        }

        //this.topContainer.style.backgroundColor = Color.red;
        //this.titleButtonContainer.style.backgroundColor = Color.green;
        //this.titleContainer.style.backgroundColor = Color.blue;
        RefreshExpandedState();
    }


    private void FetchSerializedProperty()
    {
        SerializedProperty nodes = _serializedObject.FindProperty("_nodes");
        if (nodes.isArray)
        {
            int size = nodes.arraySize;
            for (int i = 0; i < size; i++)
            {
                var element = nodes.GetArrayElementAtIndex(i);
                var elementId = element.FindPropertyRelative("_guid");

                if (elementId.stringValue == node.id)
                {
                    _serializedProperty = element;
                }
            }
        }
    }
    private PropertyField DrawProperty(string name, int rowIndex)
    {
        if (_serializedProperty == null)
        {
            FetchSerializedProperty();
        }

        SerializedProperty prop = _serializedProperty.FindPropertyRelative(name);
        PropertyField field = new PropertyField(prop);

        // If there is an inputPort connected
        if (rows[rowIndex].childCount > 0)
        {

            rows[rowIndex].Add(field);

        }
        else
        {
            rows[rowIndex].Add(field);
            inputContainer.Add(rows[rowIndex]);
        }
        return field;
    }

    void ApplyMinWidthToStructFields(SerializedProperty property, float minWidth)
    {
        SerializedProperty iterator = property.Copy();
        SerializedProperty endProperty = property.GetEndProperty();

        if (property.isArray)
            return; // Pas besoin de gérer les arrays ici, seulement les structs

        while (iterator.NextVisible(true) && !SerializedProperty.EqualContents(iterator, endProperty))
        {
            PropertyField subField = new PropertyField(iterator);
            subField.style.minWidth = minWidth;
        }
    }

    private void CreatePropertyInput(string name, Type type, int rowIndex)
    {
        Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, type);
        inputPort.portName = name;
        _ports.Add(inputPort);
        rows[rowIndex].Add(inputPort);
        inputContainer.Add(rows[rowIndex]);
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
