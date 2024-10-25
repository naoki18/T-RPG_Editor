using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeInfoAttribute : Attribute
{
    private string _nodeTitle;
    private string _menuItem;

    public string title => _nodeTitle;
    public string menuItem => _menuItem;

    public NodeInfoAttribute(string title, string menuItem = "")
    {
        _nodeTitle = title;
        _menuItem = menuItem;
    }
}
