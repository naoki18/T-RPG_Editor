using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class TileDatabaseWindow : EditorWindow
{
    TileDatabase database;
    List<ScriptableTile> searchTiles;
    string selectedTile = "";
    string search = "";
    [MenuItem("Tools/TileDatabase")]
    public static void OpenWindow()
    {
        GetWindow<TileDatabaseWindow>("TileDatabase");
    }

    private void OnGUI()
    {
        SearchInDatabase();
        if (string.IsNullOrEmpty(selectedTile) && searchTiles.Count > 0)
        {
            selectedTile = searchTiles[0].tileName;
        }

        // Window
        using (new EditorGUILayout.HorizontalScope())
        {
            // Search tile
            using(new EditorGUILayout.VerticalScope())
            {
                // Search bar
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Search :", GUILayout.Width(60));
                search = GUILayout.TextField(search, GUILayout.ExpandWidth(true));
                EditorGUILayout.EndHorizontal();

                // All tiles
                for (int i = 0; i < searchTiles.Count; i++)
                {
                    bool isSelected = searchTiles[i].tileName == selectedTile;
                    GUI.backgroundColor = isSelected ? Color.blue : new Color(0, 0, 0, 0);
                    if (GUILayout.Button(searchTiles[i].tileName, EditorStyles.selectionRect))
                    {
                        selectedTile = searchTiles[i].tileName;
                    }
                }
            }
            EditorGUILayout.LabelField("test");
        }
        
        //EditorGUILayout.BeginHorizontal();
        //GUI.color = Color.green;
        //if (GUILayout.Button("Add Item"))
        //{
        //    TileCreatorWindow.OpenWindow();
        //}
        //EditorGUILayout.EndHorizontal();
        //GUI.color = Color.white;

        //for (int i = 0; i < searchTiles.Count; i++)
        //{
        //    float yPos = 40 + 20 * i;
        //    #region TileNameBox
        //    float nameBoxWidth = 100;
        //    Vector2 nameBoxPos = new Vector2(0, yPos);
        //    Vector3 nameBoxSize = new Vector2(nameBoxWidth, 20);
        //    Rect nameRect = new Rect(nameBoxPos, nameBoxSize);
        //    string tileName = searchTiles[i].tileName;
        //    EditorGUI.LabelField(nameRect, tileName);
        //    #endregion

        //    #region EditButton
        //    float editButtonWidth = 100;
        //    Vector2 editButtonBoxPos = new Vector2(nameBoxWidth, 40 + 20 * i);
        //    Vector2 editButtonBoxSize = new Vector2(editButtonWidth, 20);
        //    Rect editButtonRect = new Rect(editButtonBoxPos, editButtonBoxSize);
        //    if (GUI.Button(editButtonRect, new GUIContent("Edit")))
        //    {
        //        TileCreatorWindow.OpenWindow(searchTiles[i]);
        //    }
        //    #endregion

        //    #region DeleteButton
        //    Vector2 deleteButtonBoxPos = new Vector2(nameBoxWidth + editButtonWidth, 40 + 20 * i);
        //    Vector2 deleteButtonBoxSize = new Vector2(100, 20);
        //    Rect deleteButtonRect = new Rect(deleteButtonBoxPos, deleteButtonBoxSize);
        //    GUI.color = Color.red;
        //    if (GUI.Button(deleteButtonRect, new GUIContent("X")))
        //    {
        //        database.RemoveTile(searchTiles[i].tileName);
        //    }
        //    GUI.color = Color.white;
        //    #endregion
        //}


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
