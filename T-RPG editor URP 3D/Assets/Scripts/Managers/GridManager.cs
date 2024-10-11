using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager instance { get; private set; }
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] Tile tilePf;

    [SerializeField] Unit c;
    Dictionary<Vector3, Tile> tileMap;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }

    public void GenerateGrid()
    {
        tileMap = new Dictionary<Vector3, Tile>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile newTile = Instantiate(tilePf, new Vector3(x, 0, y), Quaternion.identity);
                newTile.name = $"Tile {y} {x}";
                tileMap[new Vector3(x, 0, y)] = newTile;
            }
        }

        GameManager.instance.ChangeState(GameManager.GameState.PLAYER_SPAWN);
    }

    #region Getters
    public Tile GetTileAtPos(int x, int y)
    {
        return (tileMap.TryGetValue(new Vector3(x,0, y), out Tile tile) ? tile : null);
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
        pos = new Vector3(x,0, y);
        return tileMap.TryGetValue(pos, out Tile tile) ? tile : null;
    }

    public Vector3 GetRandomPos()
    {
        int x, y;
        x = Random.Range(0, width);
        y = Random.Range(0, height);
        return new Vector3(x,0, y);
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
        return new Vector3(x,0, y);
    }
    #endregion
}
