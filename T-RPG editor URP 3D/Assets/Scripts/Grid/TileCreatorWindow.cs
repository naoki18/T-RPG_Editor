using System.Collections.Generic;
using System.Data;
using UnityEditor;
using UnityEngine;

public class TileCreatorWindow : EditorWindow
{
    ScriptableTile newTile;
    private PreviewRenderUtility previewUtility;
    private GameObject targetObject;
    public static void OpenWindow()
    {
        GetWindow<TileCreatorWindow>("Tile Creator");
    }

    private void OnEnable()
    {
        newTile = CreateInstance<ScriptableTile>();
        previewUtility = new PreviewRenderUtility(); SetupPreviewScene();
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

        Rect rect = new Rect(100, 100, 200, 200);
        previewUtility.BeginPreview(rect, previewBackground: GUIStyle.none);
        previewUtility.Render(true);
        var texture = previewUtility.EndPreview();
        GUI.DrawTexture(rect, texture);

    }

    private void SetupPreviewScene()
    {
        targetObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        targetObject.transform.position = Vector3.zero;
        Debug.Log(targetObject.GetComponent<MeshRenderer>().sharedMaterial.ToString());
        // Since we want to manage this instance ourselves, hide it
        // from the current active scene, but remember to also destroy it.
        targetObject.hideFlags = HideFlags.HideAndDontSave;
        previewUtility.AddSingleGO(targetObject);
        // Camera is spawned at origin, so position is in front of the cube.
        previewUtility.camera.transform.position = new Vector3(0f, 0f, -10f);

        // This is usually set very small for good performance, but
        // we need to shift the range to something our cube can fit between.
        previewUtility.camera.nearClipPlane = 5f;
        previewUtility.camera.farClipPlane = 20f;
    }
    private void Update()
    {
        // Just do some random modifications here.
        float time = (float)EditorApplication.timeSinceStartup * 15;
        targetObject.transform.rotation = Quaternion.Euler(time * 2f, time * 4f, time * 3f);
        if (targetObject.GetComponent<MeshRenderer>().sharedMaterial != newTile.material)
        {
            targetObject.GetComponent<MeshRenderer>().sharedMaterial = newTile.material;    
        }
        Repaint();
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
