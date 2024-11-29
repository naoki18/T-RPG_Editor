using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "TileDatabase", menuName = "Database/TileDatabase")]
public class TileDatabase : ScriptableObject
{
    public List<ScriptableTile> datas;

    private void OnEnable()
    {
        foreach (var tile in Resources.LoadAll<ScriptableTile>("Tiles"))
        {
            if (!datas.Contains(tile))
            {
                datas.Add(tile);
            }
        }
    }
    public ScriptableTile GetTileData(string name)
    {
        return datas.Where(x => x.name == name).FirstOrDefault(); ;
    }

    public static TileDatabase Get()
    {
        return Resources.Load<TileDatabase>("TileDatabase");
    }

}
