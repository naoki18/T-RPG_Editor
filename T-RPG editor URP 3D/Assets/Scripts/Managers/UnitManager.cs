using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class UnitManager : MonoBehaviour
{
    public static UnitManager instance { get; private set; }

    public Unit unitPf;

    List<ScriptableUnit> units = new List<ScriptableUnit>();
    Unit selectedUnit;

    public delegate void OnSelectUnitDelegate(Unit unit);
    public event OnSelectUnitDelegate OnSelectUnit;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            units = Resources.LoadAll<ScriptableUnit>("Units").ToList();
        }
        else Destroy(this);
    }

    #region SPAWN
    public void SpawnAllies()
    {
        Unit unit = InstantiateRandomUnit(Faction.ALLY);
        if (unit == null)
        {
            GameManager.Instance.ChangeState(GameManager.GameState.ENEMIES_SPAWN);
            return;
        }
        Vector3 pos = GridManager.Instance.GetRandomValidPos();
        unit.transform.position = new Vector3(pos.x, pos.y + 1, pos.z);
        unit.SetPosition(pos);
        GameManager.Instance.ChangeState(GameManager.GameState.ENEMIES_SPAWN);
    }
    public void SpawnEnemies()
    {
        Unit unit = InstantiateRandomUnit(Faction.ENEMY);
        if (unit == null)
        {
            GameManager.Instance.ChangeState(GameManager.GameState.PLAYER_TURN);
            return;
        }

        Vector3 pos = GridManager.Instance.GetRandomValidPos();
        unit.transform.position = new Vector3(pos.x, pos.y + 1, pos.z);
        unit.SetPosition(pos);
        GameManager.Instance.ChangeState(GameManager.GameState.PLAYER_TURN);
    }
    private Unit InstantiateRandomUnit(Faction faction)
    {
        if (units.Count == 0) return null;
        var unit = units.Where(x => x.faction == faction).OrderBy(x => Random.value).First();
        if (unit == null) return null;
        return Unit.InstantiateUnit(unit);
    }
    #endregion

    #region MANAGE_UNIT
    public void SelectUnit(Unit unit)
    {
        if (unit == null) return;
        selectedUnit = unit;
        OnSelectUnit.Invoke(unit);
    }

    public IEnumerator MoveUnit(Unit unit, List<Vector3> positions)
    {
        GridManager.Instance.ClearReachablePos();
        Vector3 beginPos = positions[0];
        beginPos.y += 1;
        Vector3 currentDirection = positions[1] - positions[0];
        int range = 1;
        for (int i = 0; i < positions.Count; i++)
        {
            if (i < positions.Count - 1 && currentDirection == positions[i + 1] - positions[i])
            {
                range++;
                continue;
            }
            if(i < positions.Count - 1) currentDirection = positions[i+1] - positions[i];
            Vector3 positionToReach = positions[i];
            positionToReach.y += 1;
            float timer = 0f;
            do
            {
                unit.transform.position = Vector3.Lerp(beginPos, positionToReach, timer);
                timer += Time.deltaTime / (0.5f * (Mathf.Log(range) +0.2f) );
                timer = Mathf.Clamp01(timer);
                yield return null;
            } while (timer < 1f);
            // Call this to set the unit on the tile. Because before we only move its sprite
            unit.SetPosition(positions[i]);
            beginPos = positions[i];
            beginPos.y += 1;
            range = 1;
            yield return null;
        }
        GameManager.Instance.ChangeState(GameManager.GameState.PLAYER_TURN);
        selectedUnit = null;
        yield return null;
    }
    public void HardMoveUnit(Unit unit, Vector3 position)
    {
        unit.SetPosition(position);
    }

    public void HardMoveUnit(Unit unit, Tile tile)
    {
        Vector3 pos = new(tile.transform.position.x, tile.transform.position.y, tile.transform.position.z);
        unit.SetPosition(pos);
    }
    #endregion

    #region GETTERS
    public Unit GetSelectedUnit()
    {
        return selectedUnit;
    }
    #endregion
}
