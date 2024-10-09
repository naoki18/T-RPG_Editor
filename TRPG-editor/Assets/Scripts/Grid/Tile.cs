using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] Color baseColor;
    [SerializeField] Color oddColor;
    [SerializeField] Color highlightedColor;
    [SerializeField] SpriteRenderer sRenderer;

    Unit unitOnTile;
    bool isOdd = false;
    short walkableValue = 0;
    public void Init(int x, int y)
    {
        isOdd = ((x % 2 == 0) && (y % 2 != 0)) || ((y % 2 == 0) && (x % 2 != 0));
        SetColor(isOdd);
    }
    public void SetColor(bool isOdd)
    {
        if (isOdd) sRenderer.color = oddColor;
        else sRenderer.color = baseColor;
    }

    public void OnMouseEnter()
    {
        sRenderer.color = highlightedColor;
    }

    public void OnMouseExit()
    {
        if (unitOnTile != null && unitOnTile == UnitManager.instance.GetSelectedUnit()) return;
        SetColor(isOdd);
    }

    public void OnMouseDown()
    {
        // TODO : NE PAS UTILISER CETTE FONCTION
        if(GameManager.instance.GetState() != GameManager.GameState.PLAYER_TURN) return;
        if(unitOnTile.GetFaction() == Faction.ALLY)
        {
            UnitManager.instance.SelectUnit(unitOnTile);
        }
    }

    public void SetCharacter(Unit character)
    {
        unitOnTile = character;
    }

    public short GetWalkableValue()
    {
        return walkableValue;
    }
}
