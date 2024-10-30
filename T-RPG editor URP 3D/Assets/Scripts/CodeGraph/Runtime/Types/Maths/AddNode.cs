using System;
using UnityEngine;

[NodeInfo("Add", "Maths/Add", true)]
public class AddNode : CodeGraphNode
{
    [Input] public int _a;
    [Input] public int _b;
    [Output] public int result;

    public override string OnProcess(CodeGraphAsset graph)
    {
        result = _a+_b;
        return base.OnProcess(graph);
    }

    //public object Add(object a, object b)
    //{
    //    Debug.Log("a => " + a);
    //    if (a is short) result = (short)a + (short)b;
    //    else if (a is int) result = (int)a + (int)b;
    //    else if (a is long) result = (long)a + (long)b;
    //    else if (a is float) result = (float)a + (float)b;
    //    else if (a is double) result = (double)a + (double)b;
    //    else if (a is Vector2) result = (Vector2)a + (Vector2)b;
    //    else if (a is Vector3) result = (Vector3)a + (Vector3)b;
    //    else throw new ArgumentException($"{a.GetType()} isn't supported to be added");
    //    return result;
    //}
}
