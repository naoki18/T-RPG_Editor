using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager instance { get; private set; }
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] Tile[] tilePf;

    [SerializeField] Unit c;
    Dictionary<Vector3, Tile> tileMap;

    List<Vector3> reachablePosition = new List<Vector3>();
    Tile tileOnMouse = null;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }

    public void Start()
    {
        UnitManager.instance.OnSelectUnit += FindReachablePosition;
    }

    public void Update()
    {
        UpdateTileOnMouse();
        UpdateMouseInput();
    }
    public void GenerateGrid()
    {
        tileMap = new Dictionary<Vector3, Tile>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile newTile = Instantiate(tilePf[Random.Range(0, tilePf.Length)], new Vector3(x, 0, y), Quaternion.identity);
                newTile.name = $"Tile {y} {x}";
                tileMap[new Vector3(x, 0, y)] = newTile;
            }
        }

        GameManager.instance.ChangeState(GameManager.GameState.PLAYER_SPAWN);
    }


    #region Getters
    public Tile GetTileAtPos(int x, int y)
    {
        return (tileMap.TryGetValue(new Vector3(x, 0, y), out Tile tile) ? tile : null);
    }
    public Tile GetTileAtPos(Vector3 pos)
    {
        return (tileMap.TryGetValue(pos, out Tile tile) ? tile : null);
    }

    public Tile GetRandomTile(out Vector3 pos)
    {
        int x, y;
        x = Random.Range(0, width);
        y = Random.Range(0, height);
        pos = new Vector3(x, 0, y);
        return tileMap.TryGetValue(pos, out Tile tile) ? tile : null;
    }

    public Vector3 GetRandomPos()
    {
        int x, y;
        x = Random.Range(0, width);
        y = Random.Range(0, height);
        return new Vector3(x, 0, y);
    }

    public Vector3 GetRandomValidPos()
    {
        const int LIMIT = 1000;
        int x, y;
        int i = 0;

        do
        {
            x = Random.Range(0, width);
            y = Random.Range(0, height);
            i++;
        } while (!GetTileAtPos(x, y).IsValid() || i == LIMIT);
        if (i == LIMIT && GetTileAtPos(x, y).IsValid())
        {
            Debug.LogError("Impossible to find a valid position");
            return Vector3.zero;
        }
        return new Vector3(x, 0, y);
    }

    public bool IsReachable(Vector3 position)
    {
        return reachablePosition.Contains(position);
    }

    public bool IsReachable(Tile tile)
    {
        Vector3 pos = tile.transform.position;
        return reachablePosition.Contains(pos);
    }
    #endregion

    private void FindReachablePosition(Unit unit)
    {
        ClearReachablePos();
        reachablePosition = unit.GetReachablePos();
        HighlightReachablePosition();
    }
    private void HighlightReachablePosition()
    {
        foreach (var pos in reachablePosition)
        {
            GetTileAtPos(pos).Highlight(0.5f);
        }
    }
    public void ClearReachablePos()
    {
        foreach (var pos in reachablePosition)
        {
            GetTileAtPos(pos).RemoveHighlight();
        }
        reachablePosition.Clear();
    }

    public void UpdateTileOnMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var info))
        {
            switch(GameManager.instance.GetState())
            {
                case GameManager.GameState.PLAYER_TURN:
                    HightlightHoveredTile(info);
                    break;
                case GameManager.GameState.PLAYER_MOVE_CHARACTER:
                    UpdateHoveredTile(info);
                    break;
            }
            
        }
        Debug.DrawRay(ray.origin, ray.direction * 10, Color.red);
    }

    public void UpdateMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            switch(GameManager.instance.GetState())
            {
                case GameManager.GameState.PLAYER_TURN:
                    SelectUnit();
                    break;
                case GameManager.GameState.PLAYER_MOVE_CHARACTER:
                    SelectNewPosition();
                    break;
            }
            
        }
    }

    private void SelectUnit()
    {
        if (tileOnMouse == null) return;
        Unit characterOnTile = tileOnMouse.GetCharacter();
        if (characterOnTile != null && characterOnTile.GetFaction() == Faction.ALLY)
        {
            UnitManager.instance.SelectUnit(characterOnTile);
            GameManager.instance.ChangeState(GameManager.GameState.PLAYER_MOVE_CHARACTER);
        }
    }

    private void SelectNewPosition()
    {
        Unit selectedUnit = UnitManager.instance.GetSelectedUnit();
        if (selectedUnit != null && tileOnMouse != null && tileOnMouse.GetCharacter() == null && IsReachable(tileOnMouse))
        {
            UnitManager.instance.MoveUnit(selectedUnit, tileOnMouse);
            GameManager.instance.ChangeState(GameManager.GameState.PLAYER_TURN);
        }
    }

    private void HightlightHoveredTile(RaycastHit hit)
    {
        Vector3 position = new(Mathf.Round(hit.point.x), Mathf.Round(hit.point.y), Mathf.Round(hit.point.z));
        Tile newTile = GetTileAtPos(position);
        if (newTile != null && tileOnMouse != newTile)
        {
            if (tileOnMouse != null)
            {
                tileOnMouse.RemoveHighlight();
            }
            newTile.Highlight(0.5f);
            tileOnMouse = newTile;
        }
    }

    private void UpdateHoveredTile(RaycastHit hit)
    {
        Vector3 position = new(Mathf.Round(hit.point.x), Mathf.Round(hit.point.y), Mathf.Round(hit.point.z));
        Tile newTile = GetTileAtPos(position);
        if (newTile != null && tileOnMouse != newTile)
        {
            tileOnMouse = newTile;
        }
    }
}
