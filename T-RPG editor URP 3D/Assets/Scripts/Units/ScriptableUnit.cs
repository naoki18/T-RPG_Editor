using UnityEngine;

[CreateAssetMenu(fileName = "new unitData", menuName = "Data/UnitData")]
public class ScriptableUnit : ScriptableObject
{
    public Faction faction;
    public Sprite sprite;
    public ScriptableWeapon baseWeapon;
    public int movementPoint;
    public int hp;
}

public enum Faction
{
    ALLY,
    ENEMY
}