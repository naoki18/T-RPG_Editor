using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class UnitDatabaseWindow : EditorWindow
{
    private string search = string.Empty;
    private string selectedUnit = string.Empty;
    private ScriptableUnit selectedUnitData;
    private List<ScriptableUnit> searchUnits;
    private UnitDatabase database;
    private Vector2 scrollPos = Vector2.zero;

    private CodeGraphEditorWindow codeGraphEditorWindow = null;

    // DATAS 
    private Faction factionData;
    private Sprite spriteData;
    private ScriptableWeapon baseWeaponData;
    private int movementPointData;
    private int hpData;
    private int speedData;
    private string unitNameData;
    private CodeGraphAsset codeGraphData;

    [MenuItem("Tools/Unit Database")]
    public static void OpenWindow()
    {
        GetWindow<UnitDatabaseWindow>("Unit Database");
    }

    private void OnEnable()
    {
        database = UnitDatabase.Get();
    }

    private void OnGUI()
    {
        SearchInDatabase();
        if (string.IsNullOrEmpty(selectedUnit) && searchUnits.Count > 0)
        {
            SelectUnit(searchUnits[0].unitName);
        }
        GUILayout.Space(5);
        // Window
        using (new EditorGUILayout.HorizontalScope())
        {
            BeginWindows();
            ManageUnitPart();
            EndWindows();
            using (var dataPartWindow = new EditorGUILayout.VerticalScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    DataPart();
                    CharacterPreview();
                    CodeGraphViewPart();
                }
            }
        }
    }

    private void CharacterPreview()
    {
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            GUILayout.Label("Preview", EditorStyles.boldLabel);
            using(new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                if (spriteData != null)
                {
                    Sprite sprite = spriteData; // Ton sprite actuel
                    Texture2D texture = sprite.texture;
                    Rect rect = sprite.textureRect;

                    // Créer les coordonnées UV correctes pour l'affichage
                    Rect uvRect = new Rect(
                        rect.x / texture.width,
                        rect.y / texture.height,
                        rect.width / texture.width,
                        rect.height / texture.height
                    );
                    GUILayout.FlexibleSpace();
                    // Affichage avec la bonne portion de la texture
                    GUILayout.Box("", GUILayout.Width(rect.width * 10), GUILayout.Height(rect.height * 10));
                    GUI.DrawTextureWithTexCoords(GUILayoutUtility.GetLastRect(), texture, uvRect);
                    GUILayout.FlexibleSpace();
                }
                else
                {
                    GUILayout.Label("No sprite for this unit");
                }
            }
            using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Sprite");
                spriteData = (Sprite)EditorGUILayout.ObjectField(spriteData, typeof(Sprite), true);
                GUILayout.FlexibleSpace();
            }
        }
    }

    private void DataPart()
    {
        if (selectedUnitData == null) return;
        using (new EditorGUILayout.VerticalScope(GUILayout.Width(150)))
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Data", EditorStyles.boldLabel);
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        GUILayout.Label("name :");
                        unitNameData = EditorGUILayout.TextField(unitNameData);
                    }
                    GUI.backgroundColor = Color.red;
                    bool invalidName = string.IsNullOrEmpty(unitNameData) || unitNameData != selectedUnitData.unitName && database.GetUnitData(unitNameData) != null;
                    if (invalidName)
                    {
                        using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                        {
                            string error = string.IsNullOrEmpty(unitNameData) ? "Name can't be empty" : "Can't use this name, it already exists";
                            GUILayout.Label(error);
                        }
                    }
                    GUI.backgroundColor = Color.white;
                }
                using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                {
                    GUILayout.Label("Hp value :");
                    hpData = EditorGUILayout.IntField(hpData);
                }
                using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                {
                    GUILayout.Label("Movement point value :");
                    movementPointData = EditorGUILayout.IntField(movementPointData);
                }
                using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                {
                    GUILayout.Label("Speed value :");
                    speedData = EditorGUILayout.IntField(speedData);
                }
                using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                {
                    GUILayout.Label("Weapon :");
                    baseWeaponData = (ScriptableWeapon)EditorGUILayout.ObjectField(baseWeaponData, typeof(ScriptableWeapon), true);
                }
                using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                {
                    GUILayout.Label("Faction :");
                    factionData = (Faction)EditorGUILayout.EnumPopup(factionData);
                }
                using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                {
                    GUILayout.Label("Visual scripting :");
                    if (codeGraphData == null)
                    {
                        GUI.backgroundColor = Color.green;
                        if (GUILayout.Button("Create visual scripting"))
                        {
                            string name = $"Assets/VScripting/Units/{selectedUnitData.unitName}.asset";
                            CodeGraphAsset assetAtPath = AssetDatabase.LoadAssetAtPath<CodeGraphAsset>(name);

                            CodeGraphAsset codeGraphAsset = new();
                            if (assetAtPath == null)
                            {
                                AssetDatabase.CreateAsset(codeGraphAsset, name);
                                AssetDatabase.SaveAssets();
                            }
                            else
                            {
                                codeGraphAsset = assetAtPath;
                            }

                            codeGraphData = codeGraphAsset;
                        }
                        GUI.backgroundColor = Color.white;
                    }
                    else
                    {
                        GUI.enabled = false;
                        EditorGUILayout.ObjectField(codeGraphData, typeof(CodeGraphAsset), true);
                        GUI.enabled = true;
                        GUI.backgroundColor = Color.red;
                        if (GUILayout.Button("x"))
                        {
                            codeGraphData = null;
                        }
                        GUI.backgroundColor = Color.white;

                    }

                }
            
                if (CanSave())
                {
                    GUI.backgroundColor = Color.green;
                    if (GUILayout.Button("Save"))
                    {
                        SaveCurrentUnit();
                    }
                    GUI.backgroundColor = Color.white;
                }
            }


        }
    }

    private void CodeGraphViewPart()
    {
        if (selectedUnitData.codeGraph != null && codeGraphEditorWindow == null)
        {
            codeGraphEditorWindow = CodeGraphEditorWindow.CreateSubWindow(selectedUnitData.codeGraph, this.GetType());
            if (!codeGraphEditorWindow.docked)
                this.Dock(codeGraphEditorWindow, Docker.DockPosition.Bottom);
        }
    }

    private void ManageUnitPart()
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
                for (int i = 0; i < searchUnits.Count; i++)
                {
                    bool isSelected = searchUnits[i].unitName == selectedUnitData.name;
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        GUI.backgroundColor = isSelected ? Color.blue : new Color(1, 1, 1, 1);
                        if (GUILayout.Button(searchUnits[i].unitName))
                        {
                            SelectUnit(searchUnits[i].unitName);
                        }
                        GUI.backgroundColor = Color.red;
                        if (GUILayout.Button("X", GUILayout.Width(20)))
                        {
                            string currentUnitName = searchUnits[i].unitName;
                            database.RemoveUnit(searchUnits[i].unitName);
                            SearchInDatabase();
                            if (currentUnitName == selectedUnit)
                            {
                                selectedUnit = searchUnits.Count > 0 ? searchUnits[0].unitName : string.Empty;
                                SelectUnit(selectedUnit);
                            }

                        }
                    }
                }
            }
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("+", GUILayout.Width(200)))
            {
                database.AddUnit(new ScriptableUnit());
            }
            GUI.backgroundColor = Color.white;
        }
    }

    void SearchInDatabase()
    {
        searchUnits = database.datas.
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
        searchUnits.Sort();
    }

    private void SelectUnit(string unit)
    {
        selectedUnit = unit;
        selectedUnitData = database.GetUnitData(selectedUnit);

        factionData = selectedUnitData.faction;
        spriteData = selectedUnitData.sprite;
        baseWeaponData = selectedUnitData.baseWeapon;
        movementPointData = selectedUnitData.movementPoint;
        hpData = selectedUnitData.hp;
        speedData = selectedUnitData.speed;
        unitNameData = selectedUnitData.unitName;
        codeGraphData = selectedUnitData.codeGraph;

        if (codeGraphEditorWindow != null)
        {
            codeGraphEditorWindow.Close();
            codeGraphEditorWindow = null;
        }

        GUI.FocusControl(null);

    }

    private bool CanSave()
    {
        bool invalidName = string.IsNullOrEmpty(unitNameData) || unitNameData != selectedUnitData.unitName && database.GetUnitData(unitNameData) != null;
        return !invalidName && (factionData != selectedUnitData.faction ||
        spriteData != selectedUnitData.sprite ||
        baseWeaponData != selectedUnitData.baseWeapon ||
        movementPointData != selectedUnitData.movementPoint ||
        hpData != selectedUnitData.hp ||
        speedData != selectedUnitData.speed ||
        unitNameData != selectedUnitData.unitName ||
        codeGraphData != selectedUnitData.codeGraph);

    }

    private void SaveCurrentUnit()
    {
        selectedUnitData.faction = factionData;
        selectedUnitData.sprite = spriteData;
        selectedUnitData.baseWeapon = baseWeaponData;
        selectedUnitData.movementPoint = movementPointData;
        selectedUnitData.hp = hpData;
        selectedUnitData.speed = speedData;
        selectedUnitData.codeGraph = codeGraphData;

        if (selectedUnitData.name != unitNameData) database.RenameUnit(selectedUnitData.name, unitNameData);
        EditorUtility.SetDirty(selectedUnitData);
        EditorUtility.SetDirty(database);
    }

}
