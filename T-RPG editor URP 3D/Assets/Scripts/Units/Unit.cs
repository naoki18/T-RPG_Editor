using System;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    Faction faction;
    int movementPoint;
    Vector3 positionOnGrid;
    public static Unit InstantiateUnit(ScriptableUnit unitData)
    {
        Unit unit = Instantiate(UnitManager.instance.unitPf, Vector3.zero, Quaternion.identity);
        unit.faction = unitData.faction;
        unit.gameObject.GetComponent<SpriteRenderer>().sprite = unitData.sprite;
        unit.movementPoint = unitData.movementPoint;
        return unit;
    }

    public Faction GetFaction() { return faction; }
    public void SetPosition(Vector3 position)
    {
        Tile tile = GridManager.instance.GetTileAtPos(position);
        if (tile != null)
        {
            GridManager.instance.GetTileAtPos(this.positionOnGrid).SetCharacter(null);
            this.positionOnGrid = position;
            Vector3 wPos = this.positionOnGrid;
            wPos.y += 1;
            this.transform.position = wPos;
            tile.SetCharacter(this);
        }
        
    }

    public List<Vector3> GetReachablePos()
    {
        List<Vector3> reachablePos = new List<Vector3>();
        List<Vector3> direction = new List<Vector3>() { new Vector3(1, 0, 0), new Vector3(-1, 0, 0), new Vector3(0, 0, 1), new Vector3(0, 0, -1) };
        // Let's try every direction from the current position
        for (int i = 0; i < 4; i++)
        {
            // for each direction we have max movement point available
            int movementPointAvailable = movementPoint;
            // List with next position to try & movement point available when position has been saved
            List<Tuple<Vector3, int>> tileToTry = new() { };
            tileToTry.Add(new Tuple<Vector3, int>(positionOnGrid + direction[i], movementPointAvailable));
            do
            {
                // Get last position to try
                Tuple<Vector3, int> tuple = tileToTry[^1];
                Vector3 PositionToTry = tuple.Item1;
                movementPointAvailable = tuple.Item2;
                Tile tile = GridManager.instance.GetTileAtPos(PositionToTry);
                if (tile != null && tile.GetWalkableValue() > -1)
                {
                    movementPointAvailable -= tile.GetWalkableValue();
                    if (movementPointAvailable >= 0 && !reachablePos.Contains(PositionToTry))
                    {
                        reachablePos.Add(PositionToTry);
                    }
                    if (movementPointAvailable > 0)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            tileToTry.Add(new Tuple<Vector3, int>(PositionToTry + direction[j], movementPointAvailable));
                        }

                    }
                }
                tileToTry.Remove(tuple);
            } while (tileToTry.Count > 0);

        }
        return reachablePos;
    }
}
