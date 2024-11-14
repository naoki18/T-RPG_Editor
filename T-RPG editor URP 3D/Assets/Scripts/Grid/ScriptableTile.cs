using UnityEngine;

[CreateAssetMenu(fileName = "new tileData", menuName = "Data/TileData")]
public class ScriptableTile : ScriptableObject
{
    public string tileName;
    public Material material;
    public short walkableValue;
    public CodeGraphAsset visualScriptingAsset;
}
