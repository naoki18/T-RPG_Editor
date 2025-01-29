
using System.Collections.Generic;
using System.Linq;

public static class ListExtension
{
    public static bool Compare<T>(this List<T> lhs, List<T> rhs)
    {
        if(lhs.Count != rhs.Count) return false;
        return lhs.All(rhs.Contains);
    }
}
