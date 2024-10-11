using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager instance { get; private set; }

    [SerializeField] public Unit unitPf;

    List<ScriptableUnit> units = new List<ScriptableUnit>();
    Unit selectedUnit;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            units = Resources.LoadAll<ScriptableUnit>("Units").ToList();
        }
        else Destroy(this);
    }

    public void SpawnAllies()
    {
        Unit unit = InstantiateRandomUnit(Faction.ALLY);
        if (unit == null)
        {
            GameManager.instance.ChangeState(GameManager.GameState.ENEMIES_SPAWN);
            return;
        }
        Vector3 pos = GridManager.instance.GetRandomPos();
        unit.transform.position = new Vector3(pos.x, pos.y + 1, pos.z);
        GridManager.instance.GetTileAtPos(pos).SetCharacter(unit);
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
        
        Vector3 pos = GridManager.instance.GetRandomPos();
        unit.transform.position = new Vector3(pos.x, pos.y + 1, pos.z);
        GridManager.instance.GetTileAtPos(pos).SetCharacter(unit);
        GameManager.instance.ChangeState(GameManager.GameState.PLAYER_TURN);
    }
    private Unit InstantiateRandomUnit(Faction faction)
    {
        if (units.Count == 0) return null;
        var unit = units.Where(x => x.faction == faction).OrderBy(x => Random.value).First();
        if (unit == null) return null;
        return Unit.InstantiateUnit(unit);
    }

    public void SelectUnit(Unit unit)
    {
        if (unit == null) return;
        selectedUnit = unit;
    }

    public Unit GetSelectedUnit()
    {
        return selectedUnit;
    }
}
