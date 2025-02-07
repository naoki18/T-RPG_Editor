using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public static Vector3Int[] directions = new Vector3Int[4] { new Vector3Int(1, 0, 0), new Vector3Int(-1, 0, 0), new Vector3Int(0, 0, 1), new Vector3Int(0, 0, -1) };
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] Tile tilePf;

    List<ScriptableTile> tiles = new List<ScriptableTile>();
    Dictionary<Vector3Int, Tile> tileMap;

    List<Vector3Int> reachablePosition = new List<Vector3Int>();
    Tile tileOnMouse = null;

    public delegate void HoveringTile(Tile tile);
    public event HoveringTile OnTileHovered;
    private void Awake()
    {
        tiles = Resources.LoadAll<ScriptableTile>("Tiles").ToList();
        GameManager.OnGameStateChanged += InitializeGrid;
    }

    

    public void Start()
    {
        UnitManager.instance.OnSelectUnit += FindReachablePosition;
        //GameManager.Instance.onGameStart += GenerateGrid;
        //GameManager.Instance.onPlayerTurn += ClearReachablePos;
        OnTileHovered += UIManager.Instance.UpdateTileInfo;
    }

    private void OnDestroy()
    {
        UnitManager.instance.OnSelectUnit -= FindReachablePosition;
        //GameManager.Instance.onGameStart -= GenerateGrid;
        //GameManager.Instance.onPlayerTurn -= ClearReachablePos;
        OnTileHovered -= UIManager.Instance.UpdateTileInfo;
    }
    public void Update()
    {
        UpdateTileOnMouse();
        UpdateMouseInput();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Unit unit = UnitManager.instance.GetSelectedUnit();
            if (unit != null)
            {
                unit.Damage(10);
            }
        }
    }
    public void GenerateGrid(GameManager.GameState state)
    {
        if (state != GameManager.GameState.START_GAME) return;
        tileMap = new Dictionary<Vector3Int, Tile>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile newTile = Tile.CreateTile(tiles[Random.Range(0, tiles.Count)], tilePf);
                newTile.transform.position = new Vector3Int(x, 0, y);
                newTile.name = $"Tile {y} {x}";
                tileMap[new Vector3Int(x, 0, y)] = newTile;
            }
        }
        GameManager.Instance.SetGrid(this);
        GameManager.Instance.ChangeState(GameManager.GameState.PLAYER_SPAWN);
    }


    #region Getters
    public Tile GetTilePf()
    {
        return tilePf;
    }
    public Tile GetTileAtPos(int x, int y)
    {
        return (tileMap.TryGetValue(new Vector3Int(x, 0, y), out Tile tile) ? tile : null);
    }
    public Tile GetTileAtPos(Vector3Int pos)
    {
        if (tileMap.TryGetValue(pos, out Tile tile))
        {
            return tile;
        }
        return null;
    }

    public Tile GetRandomTile(out Vector3Int pos)
    {
        int x, y;
        x = Random.Range(0, width);
        y = Random.Range(0, height);
        pos = new Vector3Int(x, 0, y);
        return tileMap.TryGetValue(pos, out Tile tile) ? tile : null;
    }

    public Vector3Int GetRandomPos()
    {
        int x, y;
        x = Random.Range(0, width);
        y = Random.Range(0, height);
        return new Vector3Int(x, 0, y);
    }

    public Vector3Int GetRandomValidPos()
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
            return Vector3Int.zero;
        }
        return new Vector3Int(x, 0, y);
    }

    public bool IsReachable(Vector3Int position)
    {
        return reachablePosition.Contains(position);
    }

    public bool IsReachable(Tile tile)
    {
        Vector3Int pos = tile.transform.position.ToInt();
        return reachablePosition.Contains(pos);
    }
    #endregion

    #region REACHABLE_POSITION
    private void FindReachablePosition(Unit unit)
    {
        ClearReachablePos(this);
        reachablePosition = unit.GetReachablePos(this);
        HighlightReachablePosition();
    }
    private void HighlightReachablePosition()
    {
        foreach (var pos in reachablePosition)
        {
            GetTileAtPos(pos).Highlight(0.5f);
        }
    }

    public void ClearReachablePos(Grid grid)
    {
        foreach (var pos in reachablePosition)
        {
            GetTileAtPos(pos).RemoveHighlight();
        }
        reachablePosition.Clear();
    }
    #endregion

    public void UpdateTileOnMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var info))
        {
            switch (GameManager.Instance.GetState())
            {
                case GameManager.GameState.PLAYER_TURN:
                    UpdateHoveredTile(info, true);
                    break;
                case GameManager.GameState.PLAYER_MOVE_CHARACTER:
                    UpdateHoveredTile(info, false);
                    break;
            }

        }
        else if (tileOnMouse != null) SetHoveredTileNull();
        Debug.DrawRay(ray.origin, ray.direction * 10, Color.red);
    }

    public void UpdateMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            switch (GameManager.Instance.GetState())
            {
                case GameManager.GameState.PLAYER_TURN:
                    SelectUnit();
                    break;
                case GameManager.GameState.PLAYER_MOVE_CHARACTER:
                    SelectNewPosition();
                    break;
            }

        }
        if (Input.GetMouseButtonDown(1))
        {
            switch (GameManager.Instance.GetState())
            {
                case GameManager.GameState.PLAYER_MOVE_CHARACTER:
                    SelectUnit();
                    GameManager.Instance.ChangeState(GameManager.GameState.PLAYER_TURN);
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
            GameManager.Instance.ChangeState(GameManager.GameState.PLAYER_MOVE_CHARACTER);
            tileOnMouse.RemoveHighlight();
        }
    }

    private void SelectNewPosition()
    {
        Unit selectedUnit = UnitManager.instance.GetSelectedUnit();
        Tile tileOccupied = GetTileAtPos(selectedUnit.transform.position.ToInt());
        if (selectedUnit != null && tileOnMouse != null && tileOnMouse.GetCharacter() == null && IsReachable(tileOnMouse))
        {
            StartCoroutine(selectedUnit.MoveUnit(AStar.GetPath(tileOccupied, tileOnMouse, this), this));
        }
    }

    private void UpdateHoveredTile(RaycastHit hit, bool highlight = true)
    {
        Vector3Int position = hit.point.ToInt();
        Tile newTile = GetTileAtPos(position);
        if (newTile != null && tileOnMouse != newTile)
        {
            if (highlight)
            {
                if (tileOnMouse != null)
                {
                    tileOnMouse.RemoveHighlight();
                }
                newTile.Highlight(0.5f);
            }

            tileOnMouse = newTile;
            OnTileHovered?.Invoke(tileOnMouse);
        }
    }

    public static Vector3Int[] GetNeighbours(Vector3Int pos)
    {
        Vector3Int[] neighbours = new Vector3Int[4];
        for (int i = 0; i < 4; i++)
        {
            neighbours[i] = pos + directions[i];
        }

        return neighbours;
    }

    private void SetHoveredTileNull()
    {
        tileOnMouse.RemoveHighlight();
        tileOnMouse = null;
    }
}
