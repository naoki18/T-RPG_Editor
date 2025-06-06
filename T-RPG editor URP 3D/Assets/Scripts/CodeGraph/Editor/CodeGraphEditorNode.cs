using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class CodeGraphEditorNode : Node
{
    private List<Port> _ports;
    private List<VisualElement> rows;
    private CodeGraphNode _node;
    private Port _outputPort;
    private Port _inputPort;
    private SerializedObject _serializedObject;
    private SerializedProperty _serializedProperty;


    public CodeGraphNode Node => _node;
    public List<Port> Ports => _ports;


    public CodeGraphEditorNode(CodeGraphNode _node, SerializedObject codeGraphObject)
    {
        this.AddToClassList("code-graph-node");
        this._node = _node;
        _ports = new List<Port>();

        Type type = _node.GetType();
        NodeInfoAttribute info = type.GetCustomAttribute<NodeInfoAttribute>();

        title = _node.NodeName;
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

        // That's horrible but I don't have time to find a clean way to do this. It would need to rework the entire architecture
        if (type == typeof(GetComponentNode))
        {
            DrawGetComponentNode(_node, type);

            return;
        }

        else if (type == typeof(GenericNode))
        {
            DrawGenericNode(_node as GenericNode, type);

            return;
        }

        else if(type == typeof(EventNode))
        {
            DrawEventNode(_node as EventNode, type);

            return;
        }

            for (int i = 0; i < type.GetFields().Length; i++)
            {
                DrawNode(type, i);
            }

        //this.topContainer.style.backgroundColor = Color.red;
        //this.titleButtonContainer.style.backgroundColor = Color.green;
        //this.titleContainer.style.backgroundColor = Color.blue;
        RefreshExpandedState();
    }

    private void DrawGenericNode(GenericNode _node, Type type)
    {
        bool hasArgs = _node.hasArgs;
        bool hasReturn = _node.hasReturn;
        int rowNb = 0;

        for (int i = 0; i < type.GetFields().Length; i++)
        {
            FieldInfo variable = type.GetFields()[i];
            Type varType = variable.FieldType;


            if (variable.Name == "Returned")
            {
                if (!hasReturn) continue;
                varType = _node.ReturnType;
            }

            if(variable.Name == "Component")
            {
                varType = _node.ClassType;
            }

            bool hasExposedProperty = variable.GetCustomAttribute<ExposedPropertyAttribute>() != null;
            bool hasInputProperty = variable.GetCustomAttribute<InputAttribute>() != null;
            bool hasOutputAttribute = variable.GetCustomAttribute<OutputAttribute>() != null;
            if (hasExposedProperty || hasInputProperty || hasOutputAttribute)
            {
                VisualElement newRow = new VisualElement();
                newRow.style.flexDirection = FlexDirection.Row;
                rows.Add(newRow);
                if (hasInputProperty) CreatePropertyInput(variable.Name, varType, rowNb++);
                else if (hasOutputAttribute)
                {
                    CreatePropertyOutput(variable.Name, varType);
                }
                if (hasExposedProperty) DrawProperty(variable.Name, rowNb++);
            }
            
        }

        if (hasArgs)
        {
            var variable = _node.GetType().GetProperty("Args");
            List<ParamInformation> args = (List<ParamInformation>)variable.GetValue(_node);
            foreach (ParamInformation arg in args)
            {
                VisualElement newRow = new VisualElement();
                newRow.style.flexDirection = FlexDirection.Row;
                rows.Add(newRow);
                CreatePropertyInput(arg.Name, arg.Type, rowNb++);
            }
        }

        _serializedObject.ApplyModifiedProperties();
    }

    private void DrawEventNode(EventNode _node, Type type)
    {
        bool hasArgs = _node.hasArgs;
        bool hasReturn = _node.hasReturn;
        int rowNb = 0;

        for (int i = 0; i < type.GetFields().Length; i++)
        {
            FieldInfo variable = type.GetFields()[i];
            Type varType = variable.FieldType;

            if (variable.Name == "Component")
            {
                varType = _node.ClassType;
            }

            bool hasExposedProperty = variable.GetCustomAttribute<ExposedPropertyAttribute>() != null;
            bool hasInputProperty = variable.GetCustomAttribute<InputAttribute>() != null;
            bool hasOutputAttribute = variable.GetCustomAttribute<OutputAttribute>() != null;
            if (hasExposedProperty || hasInputProperty || hasOutputAttribute)
            {
                VisualElement newRow = new VisualElement();
                newRow.style.flexDirection = FlexDirection.Row;
                rows.Add(newRow);
                if (hasInputProperty) CreatePropertyInput(variable.Name, varType, rowNb++);
                else if (hasOutputAttribute)
                {
                    CreatePropertyOutput(variable.Name, varType);
                }
                if (hasExposedProperty) DrawProperty(variable.Name, rowNb++);
            }

        }

        if (hasArgs)
        {
            var variable = _node.GetType().GetProperty("Args");
            List<ParamInformation> args = (List<ParamInformation>)variable.GetValue(_node);
            foreach (ParamInformation arg in args)
            {
                VisualElement newRow = new VisualElement();
                newRow.style.flexDirection = FlexDirection.Row;
                rows.Add(newRow);
                CreatePropertyOutput(arg.Name, arg.Type);
            }
        }

        _serializedObject.ApplyModifiedProperties();
    }

    private void DrawNode(Type type, int i)
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

    private void DrawGetComponentNode(CodeGraphNode _node, Type type)
    {
        SystemTypeSerialisation.AnyTypeChanged += RefreshComponentNode;
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
                else if (hasOutputAttribute)
                {
                    CreatePropertyOutput(variable.Name, (_node as GetComponentNode).type.Type);
                }
                if (hasExposedProperty) DrawProperty(variable.Name, i);

            }
        }
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

                if (elementId.stringValue == _node.id)
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
        try
        {
            rows[rowIndex].Add(inputPort);
            inputContainer.Add(rows[rowIndex]);
        }
        catch (Exception)
        {
            UnityEngine.Debug.Log("test");
        }


    }

    private void CreatePropertyOutput(string name, Type type)
    {
        Port.Capacity capacity = type != typeof(PortType.FlowPort) ? Port.Capacity.Multi : Port.Capacity.Single;
        Port outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, capacity, type);
        outputPort.portName = name;
        outputPort.portType = type;
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

    // Horrible because really not flexible. I would rework that if I had time
    private void RefreshComponentNode()
    {
        if (_node is not GetComponentNode) return;
        _ports.First(x => x.portName == "component").portType = (_node as GetComponentNode).type.Type;
    }
}
