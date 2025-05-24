using System;
using UnityEngine;
using UnityEngine.Rendering;

[NodeInfo("Maths/Add", true)]
public class AddNode : CodeGraphNode
{
    [Input] public object _a;
    [Input] public object _b;
    [Output] public object result;

    public AddNode() : base("Add")
    {
    }

    public override string OnProcess(CodeGraphAsset graph)
    {
        result = Add(_a, _b);
        return base.OnProcess(graph);
    }


    public object Add(object a, object b)
    {
        if (a is sbyte) result = (sbyte)a + (sbyte)b;
        else if (a is byte) result = (byte)a + (byte)b;
        else if (a is short) result = (short)a + (short)b;
        else if (a is ushort) result = (ushort)a + (ushort)b;
        else if (a is int) result = (int)a + (int)b;
        else if (a is uint) result = (uint)a + (uint)b;
        else if (a is long) result = (long)a + (long)b;
        else if (a is ulong) result = (ulong)a + (ulong)b;
        else if (a is nint) result = (nint)a + (nint)b;
        else if (a is nuint) result = (nuint)a + (nuint)b;
        else if (a is float) result = (float)a + (float)b;
        else if (a is double) result = (double)a + (double)b;
        else if (a is decimal) result = (decimal)a + (decimal)b;
        else if (a is string) result = (string)a + (string)b;
        else
        {
            foreach (var test in _a.GetType().GetMethods())
            {
                if (test.Name == "op_Addition")
                {
                    object[] objects = { a, b };
                    result = test.Invoke(null, objects);
                }
            }
        }
        if (result == null) throw new Exception($"Can't find add operator for {_a.GetType()}");
        return result;
    }

}
