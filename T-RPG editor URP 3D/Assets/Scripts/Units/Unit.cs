using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Unit : MonoBehaviour
{
    Faction faction;
    int movementPoint;
    int maxHp;
    int currentHp;
    int speed;
    ScriptableWeapon weapon;

    public delegate void OnDamageDelegate(int currentHp, int maxHp);
    public event OnDamageDelegate OnDamage;

    public delegate void OnTurnDelegate(Unit character);
    public event OnTurnDelegate OnTurnEvent;

    public static Unit InstantiateUnit(ScriptableUnit unitData)
    {
        Unit unit = Instantiate(UnitManager.instance.unitPf, Vector3.zero, Quaternion.identity);
        unit.faction = unitData.faction;
        unit.gameObject.GetComponentInChildren<SpriteRenderer>().sprite = unitData.sprite;
        unit.movementPoint = unitData.movementPoint;
        unit.maxHp = unitData.hp;
        unit.currentHp = unitData.hp;
        unit.speed = unitData.speed;
        unit.weapon = unitData.baseWeapon;
        if (unitData.codeGraph)
        {
            unit.gameObject.AddComponent<CodeGraphObject>();
            unit.GetComponent<CodeGraphObject>().Asset = unitData.codeGraph;
        }
        return unit;
    }

    public Faction GetFaction() { return faction; }

    public void SetPositionOnGrid(Vector3Int position, Grid grid)
    {

        Tile tile = grid.GetTileAtPos(position);
        if (tile != null)
        {
            this.transform.position = position;
            tile.SetCharacter(this);
        }
        else
        {
            Debug.LogError("Can't find any tile at : " + position);
        }
    }

    public List<Vector3Int> GetDamageablePosition()
    {
        Grid grid = Grid.Instance;
        List<Vector3Int> toReturn = new List<Vector3Int>();
        foreach (Vector2 tile in weapon.damagedTile)
        {
            Vector3 test = this.transform.position.ToInt() + new Vector3(tile.x, this.transform.position.y, tile.y);
            if (grid.GetTileAtPos(test.ToInt()) != null)
            {
                toReturn.Add(test.ToInt());
            }

        }
        return toReturn;
    }

    public List<Tile> GetDamageableTile()
    {
        List<Tile> tiles = new();
        foreach (Vector3Int pos in GetDamageablePosition())
        {
            Tile tile = Grid.Instance.GetTileAtPos(pos);
            if (tile != null) tiles.Add(tile);
        }

        return tiles;
    }

    public List<Vector3Int> GetReachablePos()
    {
        List<Vector3Int> reachablePos = new List<Vector3Int>();
        Grid gridReference = Grid.Instance;
        // Let's try every direction from the current position
        for (int i = 0; i < 4; i++)
        {
            // for each direction we have max movement point available
            int movementPointAvailable = movementPoint;
            // List with next position to try & movement point available when position has been saved
            List<Tuple<Vector3Int, int>> tileToTry = new() { };
            tileToTry.Add(new Tuple<Vector3Int, int>(this.transform.position.ToInt() + Grid.directions[i], movementPointAvailable));
            do
            {
                // Get last position to try
                Tuple<Vector3Int, int> tuple = tileToTry[^1];
                Vector3Int PositionToTry = tuple.Item1;
                movementPointAvailable = tuple.Item2;
                Tile tile = gridReference.GetTileAtPos(PositionToTry);
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
                            tileToTry.Add(new Tuple<Vector3Int, int>(PositionToTry + Grid.directions[j], movementPointAvailable));
                        }

                    }
                }
                tileToTry.Remove(tuple);
            } while (tileToTry.Count > 0);

        }
        return reachablePos;
    }

    public int GetSpeed()
    {
        return speed;
    }

    public void Damage(int amount)
    {
        currentHp -= amount;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);
        OnDamage?.Invoke(currentHp, maxHp);

        if (currentHp <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    public IEnumerator MoveUnit(List<Vector3Int> positions)
    {
        Grid gridReference = Grid.Instance;
        gridReference.ClearReachablePos();

        if (positions.Count > 1)
        {
            Vector3 beginPos = positions[0];
            Vector3 currentDirection = positions[1] - positions[0];
            int range = 0;
            for (int i = 0; i < positions.Count; i++)
            {
                if (i < positions.Count - 1 && currentDirection == positions[i + 1] - positions[i])
                {
                    range++;
                    continue;
                }
                gridReference.GetTileAtPos(this.transform.position.ToInt()).SetCharacter(null);
                if (i < positions.Count - 1) currentDirection = positions[i + 1] - positions[i];
                Vector3 positionToReach = positions[i];
                float timer = 0f;
                do
                {
                    this.transform.position = Vector3.Lerp(beginPos, positionToReach, timer);
                    timer += (Time.deltaTime / 0.2f) / range;
                    timer = Mathf.Clamp01(timer);
                    yield return null;
                } while (timer < 1f);

                // Call this to set the unit on the tile. Because before we only move its sprite
                this.SetPositionOnGrid(Vector3Int.RoundToInt(positions[i]), gridReference);
                beginPos = positions[i];
                range = 1;
                yield return null;
            }
        }

        GameManager.GameState state = GameManager.Instance.GetState() ==
            GameManager.GameState.ENEMIES_TURN ? GameManager.GameState.PLAYER_TURN : GameManager.GameState.ENEMIES_TURN;
        GameManager.Instance.ChangeState(state);
        UnitManager.instance.UnselectUnit();

        yield return null;
    }

    public void OnTurn()
    {
        OnTurnEvent?.Invoke(this);
    }

    public int GetDamage()
    {
        return weapon.attack;
    }

    public void MoveToPlayer()
    {
        Vector3 playerPos = UnitManager.instance.Allies[0].transform.position;
        Vector3 direction = playerPos - this.transform.position;

        Tile unitTile = Grid.Instance.GetTileAtPos(this.transform.position.ToInt());
        Tile playerTile = Grid.Instance.GetTileAtPos(playerPos.ToInt());

        Vector3Int[] playerNeighbours = Grid.GetNeighbours(playerPos.ToInt());

        Vector3 closestPos = playerNeighbours[0];
        for (int i = 1; i < playerNeighbours.Length; ++i)
        {
            if (Vector3.Distance(playerNeighbours[i], this.transform.position.ToInt())
                < Vector3.Distance(closestPos, this.transform.position.ToInt()))
            {
                closestPos = playerNeighbours[i];
            }
        }

        Tile wantedTile = Grid.Instance.GetTileAtPos(closestPos.ToInt());
        List<Vector3Int> path = AStar.GetPath(unitTile, wantedTile).Take(movementPoint + 1).ToList();
        StartCoroutine(MoveUnit(path));
    }
}
