using System;
using UnityEngine;

[CreateAssetMenu(fileName = "new tileData", menuName = "Data/TileData")]
public class ScriptableTile : ScriptableObject, IComparable<ScriptableTile>
{
    public string tileName;
    public Material material;
    public short walkableValue;
    public CodeGraphAsset visualScriptingAsset;

    public int CompareTo(ScriptableTile other)
    {
        if(other == null) return 0;
        if(this.name == other.name) return 0;

        return string.Compare(this.name, other.name);
    }
}
