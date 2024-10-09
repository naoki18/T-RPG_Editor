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
        
        Vector2 pos = GridManager.instance.GetRandomPos();
        unit.transform.position = pos;
        GridManager.instance.GetTileAtPos(pos).SetCharacter(unit);
        GameManager.instance.ChangeState(GameManager.GameState.ENEMIES_SPAWN);
    }
    public void SpawnEnemies()
    {
        Unit unit = InstantiateRandomUnit(Faction.ENEMY);

        Vector2 pos = GridManager.instance.GetRandomPos();
        unit.transform.position = pos;
        GridManager.instance.GetTileAtPos(pos).SetCharacter(unit);
        GameManager.instance.ChangeState(GameManager.GameState.PLAYER_TURN);
    }
    private Unit InstantiateRandomUnit(Faction faction)
    {
        return Unit.InstantiateUnit(units.Where(x => x.faction == faction).OrderBy(x => Random.value).First());
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
