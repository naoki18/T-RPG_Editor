using UnityEditor;
using UnityEngine;

public class TileCreatorWindow : EditorWindow
{
    const float MAX_ZOOM = 20;
    const float MIN_ZOOM = 2;

    private ScriptableTile newTile;
    private PreviewRenderUtility previewUtility;
    private GameObject targetObject;

    private bool mouseDown = false;
    private bool isEditing = false;
    private string lastTileName = string.Empty;
    private float zoom = -10;

    private Vector2 lastMousePos = Vector2.zero;
    private Vector2 mouseMoveDirection = Vector2.zero;
    private Vector2 scrollDelta = Vector2.zero;

    public static void OpenWindow()
    {
        GetWindow<TileCreatorWindow>("Tile Creator");
    }
    public static void OpenWindow(ScriptableTile tile)
    {
        TileCreatorWindow window = GetWindow<TileCreatorWindow>("Tile Editing");
        window.newTile = tile;
        window.lastTileName = tile.tileName;
        window.isEditing = true;
    }
    private void OnEnable()
    {
        newTile = CreateInstance<ScriptableTile>();
        previewUtility = new PreviewRenderUtility(); 
        SetupPreviewScene();
    }

    private void OnDisable()
    {
        previewUtility.Cleanup();
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
        EditorGUILayout.Space(10);
        GUILayout.Label("Preview", EditorStyles.boldLabel);

        if(previewUtility.camera != null)
        {
            Rect rect = new Rect(100, 100, 200, 200);
            previewUtility.BeginPreview(rect, previewBackground: GUIStyle.none);
            previewUtility.Render(true);
            var texture = previewUtility.EndPreview();
            GUI.DrawTexture(rect, texture);
        }
        

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
            scrollDelta = Event.current.delta.normalized;
        }
    }

    private void SetupPreviewScene()
    {
        targetObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        targetObject.transform.position = Vector3.zero;
        // Since we want to manage this instance ourselves, hide it
        // from the current active scene, but remember to also destroy it.
        targetObject.hideFlags = HideFlags.HideAndDontSave;
        previewUtility.AddSingleGO(targetObject);
        // Camera is spawned at origin, so position is in front of the cube.
        previewUtility.camera.transform.position = new Vector3(0f, 0f, zoom);

        // This is usually set very small for good performance, but
        // we need to shift the range to something our cube can fit between.
        previewUtility.camera.nearClipPlane = MIN_ZOOM;
        previewUtility.camera.farClipPlane = MAX_ZOOM;
    }
    private void Update()
    {
        // Just do some random modifications here.
        if (mouseDown)
        {
            float time = (float)EditorApplication.timeSinceStartup * 15;
            Vector3 mouseDir = new Vector3(mouseMoveDirection.y, mouseMoveDirection.x, 0);
            targetObject.transform.rotation = Quaternion.Euler(mouseDir) * targetObject.transform.rotation;
            //targetObject.transform.position += mouseDir * Time.deltaTime;
            mouseMoveDirection = Vector2.zero;
        }
        if (scrollDelta != Vector2.zero)
        {
            Vector3 camPos = previewUtility.camera.transform.position;
            camPos.z -= scrollDelta.y;
            camPos.z = Mathf.Clamp(camPos.z, -MAX_ZOOM + 1, -MIN_ZOOM - 1);
            previewUtility.camera.transform.position = camPos;
            scrollDelta = Vector2.zero;
        }
        if (newTile.material != null && targetObject.GetComponent<MeshRenderer>().sharedMaterial != newTile.material)
        {
            targetObject.GetComponent<MeshRenderer>().sharedMaterial = newTile.material;
        }

        Repaint();
    }


    private void SaveTile()
    {
        TileDatabase database = TileDatabase.Get();


        if (newTile.name == null || (!isEditing && database.GetTileData(newTile.name) != null))
        {
            Debug.Log("name can't be null or in database");
            Close();
            return;
        }
        string folderPath = "Assets/Resources/Tiles";
        if (AssetDatabase.IsValidFolder(folderPath))
        {
            if (isEditing)
            {
                string fullPath = AssetDatabase.GenerateUniqueAssetPath(folderPath + $"/{lastTileName}.asset");
                AssetDatabase.RenameAsset(fullPath, newTile.tileName);
            }
            else
            {
                string fullPath = AssetDatabase.GenerateUniqueAssetPath(folderPath + $"/{newTile.name}.asset");
                AssetDatabase.CreateAsset(newTile, fullPath);
                database.datas.Add(newTile);
            }
                
        }
        

        Close();
    }


}
