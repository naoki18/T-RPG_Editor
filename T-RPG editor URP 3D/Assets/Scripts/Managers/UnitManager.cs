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
            GameManager.OnGameStateChanged += SpawnUnit;
        }
        else Destroy(this);
    }


    private void OnDestroy()
    {
        GameManager.Instance.onPlayerSpawn -= SpawnAllies;
        GameManager.Instance.onEnemiesSpawn -= SpawnEnemies;
    }
    #region SPAWN
   
    public void SpawnUnit(GameManager.GameState state)
    {
        switch (state)
        {
            case GameManager.GameState.PLAYER_SPAWN:
                break;
            case GameManager.GameState.ENEMIES_SPAWN:
                break;
        }
    }
    public void SpawnAllies(Grid grid)
    {
        Unit unit = InstantiateRandomUnit(Faction.ALLY);
        if (unit == null)
        {
            GameManager.Instance.ChangeState(GameManager.GameState.ENEMIES_SPAWN);
            return;
        }
        Vector3Int pos = grid.GetRandomValidPos();
        unit.SetPositionOnGrid(pos, grid);
        GameManager.Instance.ChangeState(GameManager.GameState.ENEMIES_SPAWN);
    }

    public void SpawnEnemies(Grid grid)
    {
        Unit unit = InstantiateRandomUnit(Faction.ENEMY);
        if (unit == null)
        {
            GameManager.Instance.ChangeState(GameManager.GameState.PLAYER_TURN);
            return;
        }

        Vector3Int pos = grid.GetRandomValidPos();
        unit.SetPositionOnGrid(pos, grid);
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

    public void UnselectUnit()
    {
        selectedUnit = null;
    }
    #endregion

    #region GETTERS
    public Unit GetSelectedUnit()
    {
        return selectedUnit;
    }
    #endregion
}
