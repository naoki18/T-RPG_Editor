using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDatabase", menuName = "Database/Weapon Database")]
public class WeaponDatabase : ScriptableObject
{
    public List<ScriptableWeapon> datas;

    private void Awake()
    {
        datas = new List<ScriptableWeapon>();
    }
    private void OnEnable()
    {
        foreach (var weapon in Resources.LoadAll<ScriptableWeapon>("Weapons"))
        {
            if (!datas.Contains(weapon))
            {
                datas.Add(weapon);
            }
        }
    }
    public ScriptableWeapon GetWeaponData(string name)
    {
        return datas.Where(x => x.weaponName == name).FirstOrDefault(); ;
    }

    public void RemoveWeapon(string name)
    {
        string folderPath = "Assets/Resources/Weapons";
        if (AssetDatabase.IsValidFolder(folderPath))
        {
            string fullpath = folderPath + $"/{name}.asset";
            datas.Remove(GetWeaponData(name));
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
    public static WeaponDatabase Get()
    {
        return Resources.Load<WeaponDatabase>("WeaponDatabase");
    }
    public void AddWeapon(ScriptableWeapon weapon)
    {
        WeaponDatabase database = WeaponDatabase.Get();

        if (weapon.weaponName == null)
        {
            int index = 0;
            foreach (var tileData in datas)
            {
                if (tileData.weaponName.Contains("newWeapon")) index++;
            }
            weapon.weaponName = $"newWeapon[{index}]";
        }
        weapon.name = weapon.weaponName;
        string folderPath = "Assets/Resources/Weapons";
        if (AssetDatabase.IsValidFolder(folderPath))
        {
            string fullPath = AssetDatabase.GenerateUniqueAssetPath(folderPath + $"/{weapon.weaponName}.asset");
            AssetDatabase.CreateAsset(weapon, fullPath);
            database.datas.Add(weapon);

        }
    }
    public void RenameWeapon(string weaponName, string newName)
    {
        ScriptableWeapon tileData = GetWeaponData(weaponName);
        string folderPath = "Assets/Resources/Weapons";
        if (AssetDatabase.IsValidFolder(folderPath))
        {
            string fullPath = folderPath + $"/{tileData.weaponName}.asset";
            AssetDatabase.RenameAsset(fullPath, newName);
            tileData.weaponName = newName;
            tileData.name = newName;
        }
    }
}
