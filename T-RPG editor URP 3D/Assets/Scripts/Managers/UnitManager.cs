using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
            GameManager.instance.ChangeState(GameManager.GameState.ENEMIES_SPAWN);
            return;
        }
        Vector3 pos = GridManager.instance.GetRandomValidPos();
        unit.transform.position = new Vector3(pos.x, pos.y + 1, pos.z);
        unit.SetPosition(pos);
        GameManager.instance.ChangeState(GameManager.GameState.ENEMIES_SPAWN);
    }
    public void SpawnEnemies()
    {
        Unit unit = InstantiateRandomUnit(Faction.ENEMY);
        if (unit == null)
        {
            GameManager.instance.ChangeState(GameManager.GameState.PLAYER_TURN);
            return;
        }
        
        Vector3 pos = GridManager.instance.GetRandomValidPos();
        unit.transform.position = new Vector3(pos.x, pos.y + 1, pos.z);
        unit.SetPosition(pos);
        GameManager.instance.ChangeState(GameManager.GameState.PLAYER_TURN);
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
        foreach (var position in positions)
        {
            HardMoveUnit(unit, position);
            yield return new WaitForSeconds(1f);
        }
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
