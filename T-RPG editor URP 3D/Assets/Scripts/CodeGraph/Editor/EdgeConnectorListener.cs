using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class EdgeConnectorListener : IEdgeConnectorListener
{
    CodeGraphView _currentGraphView;

    public EdgeConnectorListener(CodeGraphView view)
    {
        _currentGraphView = view;
    }
    public void OnDrop(GraphView graphView, Edge edge)
    {
        return;
    }

    public void OnDropOutsidePort(Edge edge, Vector2 position)
    {
        //Debug.Log(_currentGraphView.contentViewContainer.WorldToLocal(position));
        //_currentGraphView.ShowSearchWindow(_currentGraphView.);
    }
}