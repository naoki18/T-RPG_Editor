#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "UnitDatabase", menuName = "Database/Unit Database")]
public class UnitDatabase : ScriptableObject
{
    public List<ScriptableUnit> datas;

    private void Awake()
    {
        datas = new List<ScriptableUnit>();
    }
    private void OnEnable()
    {
        foreach (var unit in Resources.LoadAll<ScriptableUnit>("Units"))
        {
            if (!datas.Contains(unit))
            {
                datas.Add(unit);
            }
        }
    }
    public ScriptableUnit GetUnitData(string name)
    {
        return datas.Where(x => x.unitName == name).FirstOrDefault(); ;
    }

    public void RemoveUnit(string name)
    {
        string folderPath = "Assets/Resources/Units";
        if (AssetDatabase.IsValidFolder(folderPath))
        {
            string fullpath = folderPath + $"/{name}.asset";
            datas.Remove(GetUnitData(name));
            if (AssetDatabase.DeleteAsset(fullpath))
            {
                Debug.Log("<color=green>Success</color>");
            }
        }
        else
        {
            Debug.LogError($"Can't remove, path {folderPath} is invalid");
        }


    }
    public static UnitDatabase Get()
    {
        return Resources.Load<UnitDatabase>("UnitDatabase");
    }
    public void AddUnit(ScriptableUnit unit)
    {
        UnitDatabase database = UnitDatabase.Get();

        if (unit.unitName == null)
        {
            int index = 0;
            foreach (var tileData in datas)
            {
                if (tileData.unitName.Contains("newUnit")) index++;
            }
            unit.unitName = $"newUnit[{index}]";
        }
        unit.name = unit.unitName;
        string folderPath = "Assets/Resources/Units";
        if (AssetDatabase.IsValidFolder(folderPath))
        {
            string fullPath = AssetDatabase.GenerateUniqueAssetPath(folderPath + $"/{unit.unitName}.asset");
            AssetDatabase.CreateAsset(unit, fullPath);
            database.datas.Add(unit);

        }
    }
    public void RenameUnit(string unitName, string newName)
    {
        ScriptableUnit unit = GetUnitData(unitName);
        string folderPath = "Assets/Resources/Units";
        if (AssetDatabase.IsValidFolder(folderPath))
        {
            string fullPath = folderPath + $"/{unit.unitName}.asset";
            AssetDatabase.RenameAsset(fullPath, newName);
            unit.unitName = newName;
            unit.name = newName;
        }
    }
}
#endif