using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class CodeGraphView : GraphView
{
    const float MIN_ZOOM = 0.25f;
    const float MAX_ZOOM = 2f;
    private CodeGraphAsset _codeGraph;
    private SerializedObject _serializedObject;
    private CodeGraphEditorWindow _window;
    private CodeGraphWindowSearchProvider _searchWindow;

    public CodeGraphEditorWindow window => _window;
    public List<CodeGraphEditorNode> _graphNodes;
    public Dictionary<string, CodeGraphEditorNode> _nodeDict;
    public Dictionary<Edge, CodeGraphConnection> _connectionDict;

    public CodeGraphView(SerializedObject serializedObject, CodeGraphEditorWindow window)
    {
        _serializedObject = serializedObject;
        _codeGraph = (CodeGraphAsset)serializedObject.targetObject;

        _graphNodes = new List<CodeGraphEditorNode>();
        _nodeDict = new Dictionary<string, CodeGraphEditorNode>();
        _connectionDict = new Dictionary<Edge, CodeGraphConnection>();

        _searchWindow = ScriptableObject.CreateInstance<CodeGraphWindowSearchProvider>();
        _searchWindow.view = this;

        this.nodeCreationRequest += ShowSearchWindow;
        this.graphViewChanged += OnGraphViewChanged;

        SetupZoom(MIN_ZOOM, MAX_ZOOM);
        _window = window;

        StyleSheet style = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/CodeGraph/Editor/USS/CodeGraphEditor.uss");
        styleSheets.Add(style);

        GridBackground background = new GridBackground();
        background.name = "Grid";

        Add(background);
        background.SendToBack();

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        this.AddManipulator(new ClickSelector());

        DrawNodes();
        DrawConnections();

        
    }

    

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        List<Port> allPorts = new List<Port>();
        List<Port> ports = new List<Port>();
        foreach (var node in _graphNodes)
        {
            allPorts.AddRange(node.Ports);
        }
        foreach (var port in allPorts)
        {
            if (port == startPort || port.node == startPort.node || port.direction == startPort.direction) continue;
            if (port.portType == typeof(object) || port.portType == startPort.portType)
            {
                ports.Add(port);
            }
        }
        return ports;
    }
    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        if(graphViewChange.elementsToRemove != null)
        {
            RemoveNodes(graphViewChange);
            RemoveConnections(graphViewChange);
        }
        if (graphViewChange.movedElements != null)
        {
            List<CodeGraphEditorNode> movedNodes = graphViewChange.movedElements.OfType<CodeGraphEditorNode>().ToList();
            foreach (var node in movedNodes)
            {
                MoveNode(node);
            }
        }
        if(graphViewChange.edgesToCreate != null)
        {
            foreach(Edge edge in graphViewChange.edgesToCreate)
            {
                CreateEdge(edge);
            }
        }
        
        _serializedObject.Update();
        return graphViewChange;
    }

    private void RemoveConnections(GraphViewChange graphViewChange)
    {
        List<Edge> connectionsToDelete = graphViewChange.elementsToRemove.OfType<Edge>().ToList();
        foreach (var edge in connectionsToDelete)
        {
            RemoveConnection(edge);
        }
    }

    private void RemoveConnection(Edge edge)
    {
        if (!_connectionDict.TryGetValue(edge, out CodeGraphConnection toRemove)) return;
        DisconnectVariable(edge, (CodeGraphEditorNode)edge.input.node, (CodeGraphEditorNode)edge.output.node);
        _codeGraph.Connections.Remove(toRemove);
        _connectionDict.Remove(edge);
    }

    

    private void RemoveNodes(GraphViewChange graphViewChange)
    {
        List<CodeGraphEditorNode> nodesToDelete = graphViewChange.elementsToRemove.OfType<CodeGraphEditorNode>().ToList();
        foreach (var node in nodesToDelete)
        {
            RemoveNode(node);
        }
    }

    private void CreateEdge(Edge edge)
    {
        CodeGraphEditorNode inputNode = (CodeGraphEditorNode)edge.input.node;
        int inputIndex = inputNode.Ports.IndexOf(edge.input);
        CodeGraphEditorNode outputNode = (CodeGraphEditorNode)edge.output.node;
        int outputIndex = outputNode.Ports.IndexOf(edge.output);

        ConnectVariable(edge, inputNode, outputNode);

        CodeGraphConnection connection = new CodeGraphConnection(inputNode.Node.id, inputIndex, outputNode.Node.id, outputIndex);
        _codeGraph.Connections.Add(connection);
        _connectionDict.Add(edge, connection);
    }

    // Connect a variable to another during execution flow
    private void ConnectVariable(Edge edge, CodeGraphEditorNode inputNode, CodeGraphEditorNode outputNode)
    {
        System.Type inputType = inputNode.Node.GetType();
        System.Type outputType = outputNode.Node.GetType();

        FieldInfo outputField = outputType.GetField(edge.output.portName);
        FieldInfo inputField = inputType.GetField(edge.input.portName);

        
        if (outputField == null || inputField == null) return;
        if (inputField.FieldType != outputField.FieldType && inputType.GetCustomAttribute<NodeInfoAttribute>().isAllInputSameType)
        {
            foreach (var port in inputNode.Ports)
            {
                if (port.portType != typeof(PortType.FlowPort))
                    port.portType = outputField.FieldType;
            }
        }
        CodeGraphNode.LinkedValue linkedValue = new CodeGraphNode.LinkedValue(edge.input.portName, edge.output.portName, inputNode.Node.id);
        if (!outputNode.Node.linkedValues.Contains(linkedValue))
        {
            outputNode.Node.linkedValues.Add(linkedValue);
        }
        
    }

    private void DisconnectVariable(Edge edge, CodeGraphEditorNode inputNode, CodeGraphEditorNode outputNode)
    {
        System.Type inputType = inputNode.Node.GetType();
        System.Type outputType = outputNode.Node.GetType();
        outputNode.Node.linkedValues.Remove(new CodeGraphNode.LinkedValue(edge.input.portName, edge.output.portName, inputNode.Node.id));

        FieldInfo outputField = outputType.GetField(edge.output.portName);
        FieldInfo inputField = inputType.GetField(edge.input.portName);


        if (outputField == null || inputField == null) return;

        bool isAllInputSame = inputType.GetCustomAttribute<NodeInfoAttribute>().isAllInputSameType;
        bool stillHasConnectedVariable = false;
        foreach (var port in inputNode.Ports)
        {
            if(port.portType != typeof(PortType.FlowPort) && port.connected)
            {
                stillHasConnectedVariable = true;
                break;
            }
        }
        if (inputField.FieldType != outputField.FieldType && isAllInputSame && !stillHasConnectedVariable)
        {
            foreach (var port in inputNode.Ports)
            {
                if (port.portType != typeof(PortType.FlowPort))
                    port.portType = typeof(object);
            }
        }
        
    }

    private void MoveNode(CodeGraphEditorNode node)
    {
        Undo.RecordObject(_serializedObject.targetObject, "Object moved");
        node.Node.SetPosition(node.GetPosition());
    }

    private void RemoveNode(CodeGraphEditorNode node)
    {
        Undo.RecordObject(_serializedObject.targetObject, "Object removed");
        _codeGraph.Nodes.Remove(node.Node);
        _nodeDict.Remove(node.Node.id);
        _graphNodes.Remove(node);
    }

    private void DrawNodes()
    {
        foreach (CodeGraphNode node in _codeGraph.Nodes)
        {
            AddNodeToGraph(node);
        }
        Bind();
    }
    private void DrawConnections()
    {
        if (_codeGraph.Connections == null) return;
        foreach (CodeGraphConnection connection in _codeGraph.Connections)
        {
            DrawConnection(connection);
        }
    }

    private void DrawConnection(CodeGraphConnection connection)
    {
        CodeGraphEditorNode inputNode = GetNode(connection.inputPort.nodeId);
        CodeGraphEditorNode outputNode = GetNode(connection.outputPort.nodeId);
        if(inputNode == null || outputNode == null) return;

        Port inputPort = inputNode.Ports[connection.inputPort.portIndex];
        Port outputPort = outputNode.Ports[connection.outputPort.portIndex];
        Edge newConnection = inputPort.ConnectTo(outputPort);
        ConnectVariable(newConnection, inputNode, outputNode);
        AddElement(newConnection);
        _connectionDict[newConnection] = connection;
    }

    private CodeGraphEditorNode GetNode(string nodeId)
    {
        CodeGraphEditorNode node = null;
        _nodeDict.TryGetValue(nodeId, out node);
        return node;
    }


    public void ShowSearchWindow(NodeCreationContext context)
    {
        _searchWindow.target = (VisualElement)focusController.focusedElement;
        SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
    }


    public void Add(CodeGraphNode node)
    {
        Undo.RecordObject(_serializedObject.targetObject, "Added Node");

        _codeGraph.Nodes.Add(node);
        _serializedObject.Update();

        AddNodeToGraph(node);
        Bind();
    }

    private void AddNodeToGraph(CodeGraphNode node)
    {
        node.typeName = node.GetType().AssemblyQualifiedName;
        CodeGraphEditorNode editorNode = new CodeGraphEditorNode(node, _serializedObject);
        editorNode.SetPosition(node.position);
        _graphNodes.Add(editorNode);
        _nodeDict.Add(node.id, editorNode);

        AddElement(editorNode);
    }

    private void Bind()
    {
        _serializedObject.Update();
        this.Bind(_serializedObject);
    }
}
