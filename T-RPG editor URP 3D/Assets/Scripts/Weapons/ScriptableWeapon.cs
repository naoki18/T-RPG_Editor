using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new weaponData", menuName = "Data/WeaponData")]
public class ScriptableWeapon : ScriptableObject
{
    public Sprite sprite;
    public string weaponName;
    public int attack;
    public int range;
    public int width;
    public List<Vector2> damagedTile;
}
