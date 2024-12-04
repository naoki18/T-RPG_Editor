using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TileDatabaseWindow : EditorWindow
{
    TileDatabase database;
    List<ScriptableTile> searchTiles;
    string search = "";
    [MenuItem("Tools/TileDatabase")]
    public static void OpenWindow()
    {
        GetWindow<TileDatabaseWindow>("TileDatabase");
    }

    private void OnGUI()
    {
        SearchInDatabase();

        // Search Field
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Search :", GUILayout.Width(60));
        search = GUILayout.TextField(search, GUILayout.ExpandWidth(true));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUI.color = Color.green;
        if (GUILayout.Button("Add Item"))
        {
            TileCreatorWindow.OpenWindow();
        }
        EditorGUILayout.EndHorizontal();
        GUI.color = Color.white;

        for (int i = 0; i < searchTiles.Count; i++)
        {
            float yPos = 40 + 20 * i;
            float width = searchTiles[i].tileName.Length * 16;
            EditorGUI.LabelField(new Rect(new Vector2(0, yPos), new Vector2(width, 20)), searchTiles[i].tileName);
            if (GUI.Button(new Rect(new Vector2(width, 40 + 20 * i), new Vector2(100, 20)), new GUIContent("Edit")))
            {
                TileCreatorWindow.OpenWindow(searchTiles[i]);
            }
        }
    }

    private void OnEnable()
    {
        database = TileDatabase.Get();
    }
    void SearchInDatabase()
    {
        searchTiles = database.datas.
           Where(x =>
           {
               for (int i = 0; i < search.Length; i++)
               {
                   if (x.name.ToLower()[i] != search[i])
                   {
                       return false;
                   }
               }
               return true;
           }
           ).ToList();
        searchTiles.Sort();
    }
}
