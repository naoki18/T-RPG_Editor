using UnityEngine;

[CreateAssetMenu(fileName = "new unitData", menuName = "Data/UnitData")]
public class ScriptableUnit : ScriptableObject
{
    public Faction faction;
    public Sprite sprite;
    public int movementPoint;
}

public enum Faction
{
    ALLY,
    ENEMY
}