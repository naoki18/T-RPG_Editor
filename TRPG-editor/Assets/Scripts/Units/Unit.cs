using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    Faction faction;
    public static Unit InstantiateUnit(ScriptableUnit unitData)
    {
        Unit unit = Instantiate(UnitManager.instance.unitPf, Vector3.zero, Quaternion.identity);
        unit.faction = unitData.faction;
        unit.gameObject.GetComponent<SpriteRenderer>().sprite = unitData.sprite;
        return unit;
    }
}
