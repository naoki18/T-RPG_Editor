using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class CodeGraphView : GraphView
{
    private CodeGraphAsset _codeGraph;
    private SerializedObject _serializedObject;
    private CodeGraphEditorWindow _window;
    private CodeGraphWindowSearchProvider _searchWindow;
    public CodeGraphEditorWindow window => _window;

    public List<CodeGraphEditorNode> _graphNodes;
    public Dictionary<string, CodeGraphEditorNode> _nodeDict;
    public CodeGraphView(SerializedObject serializedObject, CodeGraphEditorWindow window)
    {
        _serializedObject = serializedObject;
        _codeGraph = (CodeGraphAsset)serializedObject.targetObject;
        _graphNodes = new List<CodeGraphEditorNode>();
        _nodeDict = new Dictionary<string, CodeGraphEditorNode>();
        _searchWindow = ScriptableObject.CreateInstance<CodeGraphWindowSearchProvider>();
        _searchWindow.view = this;

        this.nodeCreationRequest = ShowSearchWindow;

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
    }

    private void DrawNodes()
    {
        foreach (CodeGraphNode node in _codeGraph.Nodes)
        {
            AddNodeToGraph(node);
        }
    }

    private void ShowSearchWindow(NodeCreationContext context)
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
    }

    private void AddNodeToGraph(CodeGraphNode node)
    {
        node.typeName = node.GetType().AssemblyQualifiedName;
        CodeGraphEditorNode editorNode = new CodeGraphEditorNode(node);
        editorNode.SetPosition(node.position);
        
        _graphNodes.Add(editorNode);
        _nodeDict.Add(node.id, editorNode);

        AddElement(editorNode);
    }
}
