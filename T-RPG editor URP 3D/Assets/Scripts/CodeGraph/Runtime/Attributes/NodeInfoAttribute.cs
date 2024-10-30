using System;

public class NodeInfoAttribute : Attribute
{
    private string _nodeTitle;
    private string _menuItem;
    private bool _hasFlowInput;
    private bool _hasFlowOutput;
    private bool _isAllInputSameType;
    public string title => _nodeTitle;
    public string menuItem => _menuItem;
    public bool hasFlowInput => _hasFlowInput;
    public bool hasFlowOutput => _hasFlowOutput;
    public bool isAllInputSameType => _isAllInputSameType;

    public NodeInfoAttribute(string title, string menuItem = "", bool isAllInputSameType = false, bool hasFlowInput = true, bool hasFlowOutput = true)
    {
        _nodeTitle = title;
        _menuItem = menuItem;
        _hasFlowInput = hasFlowInput;
        _hasFlowOutput = hasFlowOutput;
        _isAllInputSameType = isAllInputSameType;
    }
}
