using UnityEngine;

[CreateAssetMenu(fileName = "new weaponData", menuName = "Data/WeaponData")]
public class ScriptableWeapon : ScriptableObject
{
    public int attack;
    public int range;
}
