using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeInfoAttribute : Attribute
{
    private string _nodeTitle;
    private string _menuItem;
    private bool _hasFlowInput;
    private bool _hasFlowOutput;

    public string title => _nodeTitle;
    public string menuItem => _menuItem;
    public bool hasFlowInput => _hasFlowInput;
    public bool hasFlowOutput => _hasFlowOutput;

    public NodeInfoAttribute(string title, string menuItem = "", bool hasFlowInput = true, bool hasFlowOutput = true)
    {
        _nodeTitle = title;
        _menuItem = menuItem;
        _hasFlowInput = hasFlowInput;
        _hasFlowOutput = hasFlowOutput;
    }
}
