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
            path.Prepend(current);
        }
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
            Vector3 current = gScore.OrderBy(x => x.Value).First().Key;
            if(current == end) return AStar.ReconstructPath(cameFrom, current);

            set.Remove(current);
            foreach (var neighbours in GridManager.GetNeighbours(current))
            {
                if (GridManager.instance.GetTileAtPos(neighbours) == null) continue;
                int neighboursGScore = gScore[current] + GridManager.instance.GetTileAtPos(neighbours).GetWalkableValue();
                if (!gScore.TryGetValue(neighbours, out _) || gScore[neighbours] < neighboursGScore)
                {
                    cameFrom[neighbours] = current;
                    gScore[neighbours] = neighboursGScore;
                    if(!set.Contains(neighbours))
                    {
                        set.Add(neighbours);
                    }
                }
            }
        }

        return new();
    }
}
