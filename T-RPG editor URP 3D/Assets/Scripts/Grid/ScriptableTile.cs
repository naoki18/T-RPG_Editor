using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new tileData", menuName = "Data/TileData")]
public class ScriptableTile : ScriptableObject
{
    public Material material;
    public short walkableValue;
}
