using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class AStar
{
    private static List<Vector3> ReconstructPath(Dictionary<Vector3, Vector3> cameFrom, Vector3 current)
    {
        List<Vector3> path = new List<Vector3>() { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, current);
        }
        // The first tile is the one where the character is on
        path.RemoveAt(0);
        return path;
    }
    public static List<Vector3> GetPath(Tile tileA, Tile tileB)
    {
        Vector3 start = tileA.transform.position;
        Vector3 end = tileB.transform.position;

        List<Vector3> set = new List<Vector3>() { start };
        Dictionary<Vector3, Vector3> cameFrom = new();

        Dictionary<Vector3, int> gScore = new();
        gScore[start] = 0;

        while (set.Count > 0)
        {
            Vector3 current = GetPosWithLowestScore(set, gScore);
            if(current == end) return AStar.ReconstructPath(cameFrom, current);

            set.Remove(current);
            foreach (var neighbour in GridManager.GetNeighbours(current))
            {
                if (GridManager.Instance.GetTileAtPos(neighbour) == null || GridManager.Instance.GetTileAtPos(neighbour).GetWalkableValue() == -1) continue;
                int neighboursGScore = gScore[current] + GridManager.Instance.GetTileAtPos(neighbour).GetWalkableValue();
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

    private static Vector3 GetPosWithLowestScore(List<Vector3> set, Dictionary<Vector3, int> dic)
    {
        Vector3 lowest = set.First();
        foreach(var pos in set)
        {
            if (dic[pos] < dic[lowest]) lowest = pos;
        }
        return lowest;
    }
}
