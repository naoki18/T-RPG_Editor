using System;
using UnityEngine;

[CreateAssetMenu(fileName = "new unitData", menuName = "Data/UnitData")]
public class ScriptableUnit : ScriptableObject, IComparable<ScriptableUnit>
{
    public Faction faction;
    public Sprite sprite;
    public ScriptableWeapon baseWeapon;
    public int movementPoint;
    public int hp;
    public int speed;
    public string unitName;
    public CodeGraphAsset codeGraph;

    public int CompareTo(ScriptableUnit other)
    {
        return other.unitName.CompareTo(unitName);
    }
}

public enum Faction
{
    ALLY,
    ENEMY
}