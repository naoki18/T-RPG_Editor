using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new unitData", menuName = "Data/UnitData")]
public class ScriptableUnit : ScriptableObject
{
    public Faction faction;
    public Sprite sprite;
}

public enum Faction
{
    ALLY,
    ENEMY
}