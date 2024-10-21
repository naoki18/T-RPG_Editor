using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static Vector3[] directions = new Vector3[4] { new Vector3(1, 0, 0), new Vector3(-1, 0, 0), new Vector3(0, 0, 1), new Vector3(0, 0, -1) };
    public static GridManager Instance { get; private set; }
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] Tile tilePf;

    List<ScriptableTile> tiles = new List<ScriptableTile>();
    Dictionary<Vector3, Tile> tileMap;

    List<Vector3> reachablePosition = new List<Vector3>();
    Tile tileOnMouse = null;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            tiles = Resources.LoadAll<ScriptableTile>("Tiles").ToList();
            Debug.Log(tiles.Count);
        }
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
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Unit unit = UnitManager.instance.GetSelectedUnit();
            if (unit != null)
            {
                unit.Damage(10);
            }
        }
    }
    public void GenerateGrid()
    {
        tileMap = new Dictionary<Vector3, Tile>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile newTile = Tile.CreateTile(tiles[Random.Range(0, tiles.Count)]);
                newTile.transform.position = new Vector3(x, 0, y);
                newTile.name = $"Tile {y} {x}";
                tileMap[new Vector3(x, 0, y)] = newTile;
            }
        }

        GameManager.Instance.ChangeState(GameManager.GameState.PLAYER_SPAWN);
    }


    #region Getters
    public Tile GetTilePf()
    {
        return tilePf;
    }
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

    #region REACHABLE_POSITION
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
    #endregion

    public void UpdateTileOnMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var info))
        {
            switch(GameManager.Instance.GetState())
            {
                case GameManager.GameState.PLAYER_TURN:
                    UpdateHoveredTile(info, true);
                    break;
                case GameManager.GameState.PLAYER_MOVE_CHARACTER:
                    UpdateHoveredTile(info, false);
                    break;
            }
            
        }
        Debug.DrawRay(ray.origin, ray.direction * 10, Color.red);
    }

    public void UpdateMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            switch(GameManager.Instance.GetState())
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
            GameManager.Instance.ChangeState(GameManager.GameState.PLAYER_MOVE_CHARACTER);
            tileOnMouse.RemoveHighlight();
        }
    }

    private void SelectNewPosition()
    {
        Unit selectedUnit = UnitManager.instance.GetSelectedUnit();
        Tile tileOccupied = GetTileAtPos(selectedUnit.GetPositionOnGrid());
        if (selectedUnit != null && tileOnMouse != null && tileOnMouse.GetCharacter() == null && IsReachable(tileOnMouse))
        {
            StartCoroutine(UnitManager.instance.MoveUnit(selectedUnit, AStar.GetPath(tileOccupied, tileOnMouse)));
        }
    }

    private void UpdateHoveredTile(RaycastHit hit, bool highlight = true)
    {
        Vector3 position = new(Mathf.Round(hit.point.x), Mathf.Round(hit.point.y), Mathf.Round(hit.point.z));
        Tile newTile = GetTileAtPos(position);
        if (newTile != null && tileOnMouse != newTile)
        {
            if(highlight)
            {
                if (tileOnMouse != null)
                {
                    tileOnMouse.RemoveHighlight();
                }
                newTile.Highlight(0.5f);
            }
            if (tileOnMouse != null) tileOnMouse.HideInformation();
            
            tileOnMouse = newTile;
            newTile.ShowInformation();
        }
    }

    public static Vector3[] GetNeighbours(Vector3 pos)
    {
        Vector3[] neighbours = new Vector3[4];
        for (int i = 0; i < 4; i++)
        {
            neighbours[i] = pos + directions[i];
        }

        return neighbours;
    }
}
