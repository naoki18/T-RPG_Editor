using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class TileDatabaseWindow : EditorWindow
{
    private const float MAX_ZOOM = 20;
    private const float MIN_ZOOM = 8;

    private bool mouseDown = false;
    private float zoom = -3f;
    private string selectedTile = "";
    private string search = "";

    private TileDatabase database;
    private ScriptableTile selectedTileData;
    private PreviewRenderUtility previewUtility;
    private GameObject targetObject;
    private List<ScriptableTile> searchTiles;

    private Vector2 mouseScrollDelta = Vector2.zero;
    private Vector2 scrollPos = Vector2.zero;
    private Vector2 lastMousePos = Vector2.zero;
    private Vector2 mouseMoveDirection = Vector2.zero;

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
            ManageTilePart();
            DataPart();
            PreviewTilePart();

        }
        MouseEvent();
    }

    private void DataPart()
    {
        if (selectedTileData == null) return;
        using (var test = new EditorGUILayout.VerticalScope(GUILayout.Width(150)))
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Data", EditorStyles.boldLabel);
            }
            using(new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("walkableValue :");
                selectedTileData.walkableValue = (short)EditorGUILayout.IntField(selectedTileData.walkableValue);
            }

        }
    }

    private void PreviewTilePart()
    {
        using (var test = new EditorGUILayout.VerticalScope(GUILayout.Width(150)))
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
            {

                GUILayout.Label("Preview", EditorStyles.boldLabel);
            }
            GUI.backgroundColor = Color.black;
            using (var previewBox = new EditorGUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.Height(150)))
            {
                // Can't make the box at defined width and height without that
                EditorGUILayout.LabelField("");
                if (previewUtility.camera != null)
                {
                    Rect rect = test.rect;
                    rect.width = 200;
                    rect.height = 200;
                    previewUtility.BeginPreview(rect, previewBackground: GUIStyle.none);
                    previewUtility.Render(true);
                    var texture = previewUtility.EndPreview();
                    GUI.DrawTexture(rect, texture);
                }
            }
            GUI.backgroundColor = Color.white;

        }
    }

    private void OnEnable()
    {
        database = TileDatabase.Get();
        previewUtility = new PreviewRenderUtility();
        SetupPreviewScene();
    }
    private void OnDisable()
    {
        previewUtility.Cleanup();
    }
    private void Update()
    {
        if (string.IsNullOrEmpty(selectedTile)) return;
        // Just do some random modifications here.
        if (mouseDown)
        {
            float time = (float)EditorApplication.timeSinceStartup * 15;
            Vector3 mouseDir = new Vector3(mouseMoveDirection.y, mouseMoveDirection.x, 0);
            targetObject.transform.rotation = Quaternion.Euler(mouseDir) * targetObject.transform.rotation;
            mouseMoveDirection = Vector2.zero;
        }
        if (mouseScrollDelta != Vector2.zero)
        {
            Vector3 camPos = previewUtility.camera.transform.position;
            camPos.z -= mouseScrollDelta.y;
            camPos.z = Mathf.Clamp(camPos.z, -MAX_ZOOM + 1, -MIN_ZOOM - 1);
            previewUtility.camera.transform.position = camPos;
            mouseScrollDelta = Vector2.zero;
        }
        selectedTileData = database.GetTileData(selectedTile);
        if (selectedTileData.material != null && targetObject.GetComponent<MeshRenderer>().sharedMaterial != selectedTileData.material)
        {
            targetObject.GetComponent<MeshRenderer>().sharedMaterial = selectedTileData.material;
        }

        Repaint();
    }

    private void SetupPreviewScene()
    {
        targetObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        targetObject.transform.position = Vector3.zero;
        targetObject.transform.eulerAngles = new Vector3(15, 20, 30);
        // Since we want to manage this instance ourselves, hide it
        // from the current active scene, but remember to also destroy it.
        targetObject.hideFlags = HideFlags.HideAndDontSave;
        previewUtility.AddSingleGO(targetObject);
        zoom = -15f;
        // Camera is spawned at origin, so position is in front of the cube.
        previewUtility.camera.transform.position = new Vector3(0f, 0f, zoom);

        // This is usually set very small for good performance, but
        // we need to shift the range to something our cube can fit between.
        previewUtility.camera.nearClipPlane = MIN_ZOOM;
        previewUtility.camera.farClipPlane = MAX_ZOOM;
    }

    private void MouseEvent()
    {
        if (Event.current.type == EventType.MouseDown)
        {
            lastMousePos = Event.current.mousePosition;
            mouseDown = true;
        }
        else if (Event.current.type == EventType.MouseUp)
        {
            mouseDown = false;
            mouseMoveDirection = Vector2.zero;
        }

        if (mouseDown && Event.current.type == EventType.MouseDrag)
        {
            Vector2 currentMousePos = Event.current.mousePosition;
            mouseMoveDirection = lastMousePos - currentMousePos;
            lastMousePos = currentMousePos;
        }

        if (Event.current.type == EventType.ScrollWheel)
        {
            mouseScrollDelta = Event.current.delta.normalized;
        }
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

    void ManageTilePart()
    {
        using (new EditorGUILayout.VerticalScope(GUILayout.Width(200)))
        {
            // Search bar
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Search :", GUILayout.Width(60));
            search = GUILayout.TextField(search);
            EditorGUILayout.EndHorizontal();

            //Scroll bar
            using (new EditorGUILayout.ScrollViewScope(scrollPos))
            {
                for (int i = 0; i < searchTiles.Count; i++)
                {
                    bool isSelected = searchTiles[i].tileName == selectedTile;
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        GUI.backgroundColor = isSelected ? Color.blue : new Color(1, 1, 1, 1);
                        if (GUILayout.Button(searchTiles[i].tileName))
                        {
                            selectedTile = searchTiles[i].tileName;
                        }
                        GUI.backgroundColor = Color.red;
                        if (GUILayout.Button("X", GUILayout.Width(20)))
                        {
                            if (searchTiles[i].tileName == selectedTile)
                            {
                                selectedTile = searchTiles.Count > 0 ? searchTiles[0].tileName : string.Empty;
                            }
                            database.RemoveTile(searchTiles[i].tileName);
                        }
                    }

                }
            }
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("+", GUILayout.Width(200)))
            {
                database.AddTile(new ScriptableTile());
            }
            GUI.backgroundColor = Color.white;
        }

    }
}
