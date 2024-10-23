using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;

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
