using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CodeGraphEditorNode : Node
{
    public CodeGraphEditorNode()
    {
        this.AddToClassList("code-graph-node");
    }
}
