using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "new weaponData", menuName = "Data/WeaponData")]
public class ScriptableWeapon : ScriptableObject, IComparable<ScriptableWeapon>
{
    public Sprite sprite;
    public string weaponName;
    public int attack;
    public int range;
    public List<Vector2> damagedTile;

    public int CompareTo(ScriptableWeapon other)
    {
        if (other == null) return 0;
        if (this.weaponName == other.weaponName) return 0;

        return string.Compare(this.weaponName, other.weaponName);
    }
}
