using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "TileDatabase", menuName = "Database/TileDatabase")]
public class TileDatabase : ScriptableObject
{
    public List<ScriptableTile> datas;

    public ScriptableTile GetTileData(string name)
    {
        return datas.Where(x => x.name == name).FirstOrDefault(); ;
    }

    public static TileDatabase Get()
    {
        return Resources.Load<TileDatabase>("TileDatabase");
    }

}
