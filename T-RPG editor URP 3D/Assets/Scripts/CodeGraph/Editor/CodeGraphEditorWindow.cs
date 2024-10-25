using System;
using UnityEditor;
using UnityEditor.Build.Content;
using UnityEngine;


public class CodeGraphEditorWindow : EditorWindow
{
    [SerializeField] private CodeGraphAsset graph;
    public CodeGraphAsset currentGraph => graph;
    [SerializeField] private CodeGraphView view;
    [SerializeField] private SerializedObject serializedObject;

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

    public void Load(CodeGraphAsset target)
    {
        graph = target;
        DrawGraph();
    }

    private void DrawGraph()
    {
        serializedObject = new SerializedObject(graph);
        view = new CodeGraphView(serializedObject, this);
        rootVisualElement.Add(view);
    }
}
