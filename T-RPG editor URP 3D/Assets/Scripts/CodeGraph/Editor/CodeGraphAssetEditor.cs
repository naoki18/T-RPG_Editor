using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CodeGraphAsset))]
public class CodeGraphAssetEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if(GUILayout.Button("Edit"))
        {
            CodeGraphEditorWindow.Open((CodeGraphAsset)target);
        }
    }
}
