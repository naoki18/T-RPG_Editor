using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class WeaponDatabaseWindow : EditorWindow
{
    private const float MAX_ZOOM = 20;
    private const float MIN_ZOOM = 8;

    private bool mouseDown = false;
    private float zoom = -3f;
    private string selectedWeapon = "";
    private string search = "";

    // DATAS

    public Sprite spriteData;
    private string nameData = "";
    public int attackData;
    public int rangeData;
    public int widthData;
    public List<Vector2> damagedTileData;

    private WeaponDatabase database;
    private ScriptableWeapon selectedWeaponData;
    private GameObject targetObject;
    private List<ScriptableWeapon> searchWeapons;

    private Vector2 scrollPos = Vector2.zero;
    public Rect windowRect = new Rect(100, 100, 200, 200);
    [MenuItem("Tools/Weapon Database")]
    public static void OpenWindow()
    {
        GetWindow<WeaponDatabaseWindow>("Weapon Database");
    }

    private void OnGUI()
    {
        SearchInDatabase();
        if (string.IsNullOrEmpty(selectedWeapon) && searchWeapons.Count > 0)
        {
            SelectWeapon(searchWeapons[0].weaponName);

        }
        GUILayout.Space(5);
        // Window
        using (new EditorGUILayout.HorizontalScope())
        {
            BeginWindows();
            ManageWeaponPart();
            EndWindows();
            using (var dataPartWindow = new EditorGUILayout.VerticalScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    DataPart();
                    AttackDesignPart();
                }
            }
        }
    }
    private void DataPart()
    {
        if (selectedWeaponData == null) return;
        using (var test = new EditorGUILayout.VerticalScope(GUILayout.Width(150)))
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Data", EditorStyles.boldLabel);
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        GUILayout.Label("name :");
                        nameData = EditorGUILayout.TextField(nameData);
                    }
                    GUI.backgroundColor = Color.red;
                    bool invalidName = string.IsNullOrEmpty(nameData) || nameData != selectedWeaponData.weaponName && database.GetWeaponData(nameData) != null;
                    if (invalidName)
                    {
                        using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                        {
                            string error = string.IsNullOrEmpty(nameData) ? "Name can't be empty" : "Can't use this name, it already exists";
                            GUILayout.Label(error);
                        }
                    }
                    GUI.backgroundColor = Color.white;
                }
                using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                {
                    GUILayout.Label("Attack value :");
                    attackData = (short)EditorGUILayout.IntField(attackData);
                }
                using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                {
                    GUILayout.Label("Range value :");
                    rangeData = (short)EditorGUILayout.IntField(rangeData);
                }
                using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                {
                    GUILayout.Label("Width value :");
                    widthData = (short)EditorGUILayout.IntField(widthData);
                }
                using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                {
                    GUILayout.Label("Sprite :");
                    spriteData = (Sprite)EditorGUILayout.ObjectField(spriteData, typeof(Sprite), true);
                }
                if (CanSave())
                {
                    GUI.backgroundColor = Color.green;
                    if (GUILayout.Button("Save"))
                    {
                        SaveCurrentWeapon();
                    }
                    GUI.backgroundColor = Color.white;
                }
            }


        }
    }

    private void AttackDesignPart()
    {
        if (selectedWeaponData == null) return;
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            GUILayout.Label("Attack design", EditorStyles.boldLabel);
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                for (int i = 0; i < selectedWeaponData.range; i++)
                {
                    using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                    {
                        GUILayout.FlexibleSpace();
                        for (int j = 0; j < selectedWeaponData.width * 2 - 1; j++)
                        {
                            Vector2 pos = new Vector2(i, j);
                            bool isSelected = damagedTileData.Contains(pos);
                            if (isSelected) GUI.color = Color.green;
                            if (GUILayout.Button("", GUILayout.Width(75)))
                            {
                                if (isSelected) damagedTileData.Remove(pos);
                                else damagedTileData.Add(pos);
                            }
                            GUI.color = Color.white;
                        }
                        GUILayout.FlexibleSpace();
                    }
                }
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    GUIStyle style = new GUIStyle(EditorStyles.helpBox);
                    style.alignment = TextAnchor.MiddleCenter;
                    style.fontSize = 15;
                    GUI.color = Color.yellow;
                    GUILayout.Label("Player", style, GUILayout.Width(75));
                    GUI.color = Color.white;
                    GUILayout.FlexibleSpace();
                }



                GUI.backgroundColor = Color.white;
            }
        }
    }





    private void OnEnable()
    {
        database = WeaponDatabase.Get();
    }



    void SearchInDatabase()
    {
        searchWeapons = database.datas.
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
        searchWeapons.Sort();
    }

    void ManageWeaponPart()
    {
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.Width(200)))
        {
            // Search bar
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Search :", EditorStyles.boldLabel, GUILayout.Width(60));
            search = GUILayout.TextField(search);
            EditorGUILayout.EndHorizontal();

            //Scroll bar
            using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPos))
            {
                scrollPos = scrollView.scrollPosition;
                for (int i = 0; i < searchWeapons.Count; i++)
                {
                    bool isSelected = searchWeapons[i].weaponName == selectedWeaponData.name;
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        GUI.backgroundColor = isSelected ? Color.blue : new Color(1, 1, 1, 1);
                        if (GUILayout.Button(searchWeapons[i].weaponName))
                        {
                            SelectWeapon(searchWeapons[i].weaponName);
                        }
                        GUI.backgroundColor = Color.red;
                        if (GUILayout.Button("X", GUILayout.Width(20)))
                        {
                            string currentWeaponName = searchWeapons[i].weaponName;
                            database.RemoveWeapon(searchWeapons[i].weaponName);
                            SearchInDatabase();
                            if (currentWeaponName == selectedWeapon)
                            {
                                selectedWeapon = searchWeapons.Count > 0 ? searchWeapons[0].weaponName : string.Empty;
                                SelectWeapon(selectedWeapon);
                            }
                            
                        }
                    }
                }
            }
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("+", GUILayout.Width(200)))
            {
                database.AddWeapon(new ScriptableWeapon());
            }
            GUI.backgroundColor = Color.white;
        }

    }

    private void SelectWeapon(string weapon)
    {
        selectedWeapon = weapon;
        selectedWeaponData = database.GetWeaponData(selectedWeapon);

        attackData = selectedWeaponData.attack;
        rangeData = selectedWeaponData.range;
        widthData = selectedWeaponData.width;
        nameData = selectedWeaponData.name;
        spriteData = selectedWeaponData.sprite;
        damagedTileData = new List<Vector2>();
        // Doing for loop to avoid ref value
        for (int i = 0; i < selectedWeaponData.damagedTile.Count; i++) damagedTileData.Add(selectedWeaponData.damagedTile[i]);

        GUI.FocusControl(null);

    }
    private bool CanSave()
    {
        bool invalidName = string.IsNullOrEmpty(nameData) || nameData != selectedWeaponData.weaponName && database.GetWeaponData(nameData) != null;
        return !invalidName && (attackData != selectedWeaponData.attack ||
            nameData != selectedWeaponData.name ||
            widthData != selectedWeaponData.width ||
            rangeData != selectedWeaponData.range ||
            spriteData != selectedWeaponData.sprite ||
            !damagedTileData.Compare(selectedWeaponData.damagedTile));

    }

    private void SaveCurrentWeapon()
    {
        selectedWeaponData.sprite = spriteData;
        selectedWeaponData.attack = attackData;
        selectedWeaponData.range = rangeData;
        selectedWeaponData.width = widthData;
        selectedWeaponData.damagedTile = new List<Vector2>();
        // avoid ref value
        for (int i = 0; i < damagedTileData.Count; i++) selectedWeaponData.damagedTile.Add(damagedTileData[i]);

        if (selectedWeaponData.name != nameData) database.RenameWeapon(selectedWeaponData.name, nameData);
        EditorUtility.SetDirty(selectedWeaponData);
        EditorUtility.SetDirty(database);
    }
}
