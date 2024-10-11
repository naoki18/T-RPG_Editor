using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    Faction faction;
    int movementPoint;
    Vector2 position;
    public static Unit InstantiateUnit(ScriptableUnit unitData)
    {
        Unit unit = Instantiate(UnitManager.instance.unitPf, Vector3.zero, Quaternion.identity);
        unit.faction = unitData.faction;
        unit.gameObject.GetComponent<SpriteRenderer>().sprite = unitData.sprite;
        unit.movementPoint = unitData.movementPoint;
        return unit;
    }

    public Faction GetFaction() { return faction; }
    public void SetPosition(Vector2 position)
    {
        this.position = position;
    }

    public List<Vector2> GetReachablePos()
    {
        List<Vector2> reachablePos = new List<Vector2>();
        List<Vector2> direction = new List<Vector2> { new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, 1), new Vector2(0,-1) };
        int movementPointAvailable = movementPoint;
        for (int i = 0; i < 4; i++)
        {

        }
        return reachablePos;
    }
}
