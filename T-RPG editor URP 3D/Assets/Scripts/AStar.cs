using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class AStar
{
    private static List<Vector3Int> ReconstructPath(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int current)
    {
        List<Vector3Int> path = new List<Vector3Int>() { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, current);
        }
        // The first tile is the one where the character is on
        //path.RemoveAt(0);
        return path;
    }
    public static List<Vector3Int> GetPath(Tile tileA, Tile tileB)
    {
        Vector3Int start = tileA.transform.position.ToInt();
        Vector3Int end = tileB.transform.position.ToInt();

        List<Vector3Int> set = new List<Vector3Int>() { start };
        Dictionary<Vector3Int, Vector3Int> cameFrom = new();

        Dictionary<Vector3Int, int> gScore = new();
        gScore[start] = 0;

        while (set.Count > 0)
        {
            Vector3Int current = GetPosWithLowestScore(set, gScore);
            if(current == end) return AStar.ReconstructPath(cameFrom, current);

            set.Remove(current);
            foreach (var neighbour in Grid.GetNeighbours(current))
            {
                if (Grid.Instance.GetTileAtPos(neighbour) == null || Grid.Instance.GetTileAtPos(neighbour).GetWalkableValue() == -1) continue;
                int neighboursGScore = gScore[current] + Grid.Instance.GetTileAtPos(neighbour).GetWalkableValue();
                if (!gScore.TryGetValue(neighbour, out _) || gScore[neighbour] > neighboursGScore)
                {
                    cameFrom[neighbour] = current;
                    gScore[neighbour] = neighboursGScore;
                    if(!set.Contains(neighbour))
                    {
                        set.Add(neighbour);
                    }
                }
            }
        }

        return new();
    }

    private static Vector3Int GetPosWithLowestScore(List<Vector3Int> set, Dictionary<Vector3Int, int> dic)
    {
        Vector3Int lowest = set.First();
        foreach(var pos in set)
        {
            if (dic[pos] < dic[lowest]) lowest = pos;
        }
        return lowest;
    }
}
