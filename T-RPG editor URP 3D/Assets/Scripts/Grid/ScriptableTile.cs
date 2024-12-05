using System;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "new tileData", menuName = "Data/TileData"), ScriptableOf(typeof(Tile))]
public class ScriptableTile : ScriptableObject, IComparable<ScriptableTile>
{
    public string tileName;
    public short walkableValue;
    public Material material;
    public CodeGraphAsset visualScriptingAsset;

    public int CompareTo(ScriptableTile other)
    {
        if(other == null) return 0;
        if(this.name == other.name) return 0;

        return string.Compare(this.name, other.name);
    }
}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(ScriptableTile))]
public class TileEditor : UnityEditor.Editor
{
    Editor gameObjectEditor;
    GameObject gameObject;
    public override void OnPreviewGUI(Rect r, GUIStyle background)
    {
        base.OnPreviewGUI(r, background);
       
    }
}
#endif
