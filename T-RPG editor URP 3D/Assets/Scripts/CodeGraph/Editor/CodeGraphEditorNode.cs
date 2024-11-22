using log4net.Filter;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Rendering.FilterWindow;

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

        for (int i = 0; i < type.GetFields().Length; i++)
        {
            FieldInfo variable = type.GetFields()[i];
            bool hasExposedProperty = variable.GetCustomAttribute<ExposedPropertyAttribute>() != null;
            bool hasInputProperty = variable.GetCustomAttribute<InputAttribute>() != null;
            bool hasOutputAttribute = variable.GetCustomAttribute<OutputAttribute>() != null;
            if (hasExposedProperty || hasInputProperty || hasOutputAttribute)
            {
                VisualElement newRow = new VisualElement();
                newRow.style.flexDirection = FlexDirection.Row;
                rows.Add(newRow);
                if (hasInputProperty) CreatePropertyInput(variable.Name, variable.FieldType, i);
                else if (hasOutputAttribute) CreatePropertyOutput(variable.Name, variable.FieldType);
                if (hasExposedProperty) DrawProperty(variable.Name, i);

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
            field.label = "";
            rows[rowIndex].Add(field);

        }
        else
        {
            rows[rowIndex].Add(field);
            inputContainer.Add(rows[rowIndex]);
        }
        field.name = name;
        return field;
    }

    public void RebindProperties(int index)
    {
        for (int i = 0; i < rows.Count; i++)
        {
            foreach (var content in rows[i].Children())
            {
                if (content.GetType() == typeof(PropertyField))
                {
                    SerializedProperty prop = _serializedProperty.FindPropertyRelative(content.name);
                    UnityEngine.Debug.Log(prop.propertyPath);
                    (content as PropertyField).bindingPath = $"_nodes.Array.data[{index}].{content.name}";
                }
            }
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

    // Used to remove property field when a variable is already connected to
    public void RemoveLitteralProperty(Edge edge)
    {
        if (edge == null) return;
        foreach (var row in rows)
        {
            if (row.Contains(edge.input))
            {
                foreach (var visualElement in row.Children())
                {
                    if (visualElement is PropertyField)
                    {
                        row.Remove(visualElement);
                        break;
                    }
                }
                break;
            }
        }
    }
    public void AddLitteralProperty(Edge edge)
    {
        if (edge == null) return;
        for (int i = 0; i < rows.Count; i++)
        {
            if (rows[i].Contains(edge.input))
            {
                //DrawProperty(edge.input.portName, i);
                if (_serializedProperty == null)
                {
                    FetchSerializedProperty();
                }
                SerializedProperty prop = _serializedProperty.FindPropertyRelative(edge.input.portName);
                PropertyField field = new PropertyField(prop);

                field.label = "";
                rows[i].Add(field);

                break;
            }
        }

    }
}
