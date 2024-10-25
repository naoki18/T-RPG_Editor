using System;
using System.Reflection;
using UnityEditor.Experimental.GraphView;

public class CodeGraphEditorNode : Node
{
    private CodeGraphNode node;
    public CodeGraphEditorNode(CodeGraphNode _node)
    {
        this.AddToClassList("code-graph-node");
        this.node = _node;

        Type type = _node.GetType();
        NodeInfoAttribute info = type.GetCustomAttribute<NodeInfoAttribute>();

        string[] depth = info.menuItem.Split("/");
        foreach (string str in depth)
        {
            this.AddToClassList(str.ToLower().Replace(' ', '-'));
        }

        title = info.title;
    }
}
