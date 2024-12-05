using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class CodeGraphCreationEditor : EditorWindow
{
    [MenuItem("Tools/CreateCodeGraph")]
    public static void OpenWindow()
    {
        GetWindow<CodeGraphCreationEditor>("TileDatabase");
    }

    private void OnGUI()
    {
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
#endif