using UnityEngine;

public static class Vector3Extension
{
    public static Vector3Int ToInt(this Vector3 vector)
    {
        return Vector3Int.RoundToInt(vector);
    }
}
