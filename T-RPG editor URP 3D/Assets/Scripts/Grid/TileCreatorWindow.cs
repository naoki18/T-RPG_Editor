using UnityEditor;
using UnityEngine;

public class TileCreatorWindow : EditorWindow
{
    ScriptableTile newTile;
    public static void OpenWindow()
    {
        GetWindow<TileCreatorWindow>("Tile Creator");
    }

    private void OnEnable()
    {
        newTile = CreateInstance<ScriptableTile>();
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("name : ");
        newTile.tileName = GUILayout.TextField(newTile.tileName);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("material : ");
        newTile.material = EditorGUILayout.ObjectField(newTile.material, typeof(Material), true) as Material;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("walkableValue : ");
        newTile.walkableValue = (short)EditorGUILayout.IntField(newTile.walkableValue);
        EditorGUILayout.EndHorizontal();
        newTile.walkableValue = (short)Mathf.Clamp(newTile.walkableValue, short.MinValue, short.MaxValue);

        newTile.name = newTile.tileName;

        if (GUILayout.Button("Save"))
        {
            SaveTile();
        }
    }

    private void SaveTile()
    {
        TileDatabase database = TileDatabase.Get();
        if (newTile.name == null || database.GetTileData(newTile.name) != null)
        {
            Debug.Log("Close");
            return;
        }
        string folderPath = "Assets/Resources/Tiles";
        if (AssetDatabase.IsValidFolder(folderPath))
        {
            string fullPath = AssetDatabase.GenerateUniqueAssetPath(folderPath + $"/{newTile.name}.asset");
            AssetDatabase.CreateAsset(newTile, fullPath);
        }
        database.datas.Add(newTile);
        Close();
    }
}
