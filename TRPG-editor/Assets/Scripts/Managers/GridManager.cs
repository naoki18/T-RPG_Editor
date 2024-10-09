using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class GridManager : MonoBehaviour
{
    public static GridManager instance { get; private set; }

    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] Tile tilePf;

    [SerializeField] Unit c;
    Dictionary<Vector2, Tile> tileMap;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }

    public void GenerateGrid()
    {
        tileMap = new Dictionary<Vector2, Tile>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile newTile = Instantiate(tilePf, new Vector3(x, y), Quaternion.identity);
                newTile.name = $"Tile {y} {x}";
                newTile.Init(x, y);
                tileMap[new Vector2(x, y)] = newTile;
            }
        }

        Camera.main.transform.position = new Vector3((float)width / 2 - 0.5f, (float)height / 2 - 0.5f, -10);
        GameManager.instance.ChangeState(GameManager.GameState.PLAYER_SPAWN);
    }

    #region Getters
    public Tile GetTileAtPos(int x, int y)
    {
        return (tileMap.TryGetValue(new Vector2(x, y), out Tile tile) ? tile : null);
    }
    public Tile GetTileAtPos(Vector2 pos)
    {
        return (tileMap.TryGetValue(pos, out Tile tile) ? tile : null);
    }

    public Tile GetRandomTile(out Vector2 pos)
    {
        int x, y;
        x = Random.Range(0, width);
        y = Random.Range(0, height);
        pos = new Vector2(x, y);
        return tileMap.TryGetValue(pos, out Tile tile) ? tile : null;
    }

    public Vector2 GetRandomPos()
    {
        int x, y;
        x = Random.Range(0, width);
        y = Random.Range(0, height);
        return new Vector2(x, y);
    }
    #endregion
}
