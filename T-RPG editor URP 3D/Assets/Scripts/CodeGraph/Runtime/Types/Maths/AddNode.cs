using System;
using UnityEngine;
using UnityEngine.Rendering;

[NodeInfo("Add", "Maths/Add", true)]
public class AddNode : CodeGraphNode
{
    [Input] public object _a;
    [Input] public object _b;
    [Output] public object result;

    public override string OnProcess(CodeGraphAsset graph)
    {
        result = Add(_a,_b);
        return base.OnProcess(graph);
    }

    public object Add(object a, object b)
    {
        if (a is short) result = (short)a + (short)b;
        else if (a is int) result = (int)a + (int)b;
        else if (a is long) result = (long)a + (long)b;
        else if (a is float) result = (float)a + (float)b;
        else if (a is double) result = (double)a + (double)b;
        else if (a is Vector2) result = (Vector2)a + (Vector2)b;
        else if (a is Vector3) result = (Vector3)a + (Vector3)b;
        else throw new ArgumentException($"{a.GetType()} isn't supported to be added");
        return result;
    }
}
