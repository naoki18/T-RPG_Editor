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

    public void RemoveTile(string name)
    {
        string folderPath = "Assets/Resources/Tiles";
        if(AssetDatabase.IsValidFolder(folderPath))
        {
            string fullpath = folderPath + $"/{name}.asset";
            datas.Remove(GetTileData(name));
            if(AssetDatabase.DeleteAsset(fullpath))
            {
                Debug.Log("<color=green>Success</color>");
            }
        }
        else
        {
            Debug.LogError($"Can't remove, path {folderPath} is invalid");
        }
        
        
    }
    public static TileDatabase Get()
    {
        return Resources.Load<TileDatabase>("TileDatabase");
    }
    public void AddTile(ScriptableTile tile)
    {
        TileDatabase database = TileDatabase.Get();

        if (tile.tileName == null)
        {
            int index = 0;
            foreach (var tileData in datas) {
                if (tileData.tileName.Contains("newTile")) index++;
            }
            tile.tileName = $"newTile[{index}]";
        }
        tile.name = tile.tileName;
        string folderPath = "Assets/Resources/Tiles";
        if (AssetDatabase.IsValidFolder(folderPath))
        {
                string fullPath = AssetDatabase.GenerateUniqueAssetPath(folderPath + $"/{tile.tileName}.asset");
                AssetDatabase.CreateAsset(tile, fullPath);
                database.datas.Add(tile);

        }
    }
}
