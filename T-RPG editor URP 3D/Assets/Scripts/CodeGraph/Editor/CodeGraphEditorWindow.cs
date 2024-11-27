using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


public class CodeGraphEditorWindow : EditorWindow
{
    [SerializeField] private CodeGraphAsset graph;
    [SerializeField] private CodeGraphView view;
    [SerializeField] private SerializedObject serializedObject;

    public CodeGraphAsset currentGraph => graph;
    public static void Open(CodeGraphAsset target)
    {
        CodeGraphEditorWindow[] windows = Resources.FindObjectsOfTypeAll<CodeGraphEditorWindow>();
        foreach(var window in windows)
        {
            if(window.currentGraph == target)
            {
                window.Focus();
                return;
            }
        }
        CodeGraphEditorWindow newWindow = CreateWindow<CodeGraphEditorWindow>(typeof(CodeGraphEditorWindow), typeof(SceneView));
        newWindow.titleContent = new GUIContent($"{target.name}", EditorGUIUtility.ObjectContent(null, typeof(CodeGraphAsset)).image);
        newWindow.Load(target);
    }

    private void OnEnable()
    {
        if (graph != null)
        {
            DrawGraph();
        }
    }
    public void OnGUI()
    {
        if(EditorUtility.IsDirty(currentGraph))
        {
            this.hasUnsavedChanges = true;
        }
        else
        {
            this.hasUnsavedChanges = false;
        }
    }
    public void Load(CodeGraphAsset target)
    {
        graph = target;
        DrawGraph();
    }

    private void DrawGraph()
    {
        serializedObject = new SerializedObject(graph);
        view = new CodeGraphView(serializedObject, this);
        view.graphViewChanged += OnChange;
        rootVisualElement.Add(view);
    }

    private GraphViewChange OnChange(GraphViewChange graphViewChange)
    {
        EditorUtility.SetDirty(currentGraph);
        return graphViewChange;
    }
}
