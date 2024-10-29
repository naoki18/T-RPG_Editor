using System;
using System.Runtime.CompilerServices;
using UnityEngine;
public class CodeGraphObject : MonoBehaviour
{
    [SerializeField]
    private CodeGraphAsset _asset;

    // This is to modify a new asset and not the main scriptable
    private CodeGraphAsset assetInstance;

    private CodeGraphNode startNode;
    private CodeGraphNode updateNode;
    private void Start()
    {
        assetInstance = Instantiate(_asset);
        InitGraph();
        ProcessAndMoveToNextNode(startNode);
    }

    private void InitGraph()
    {
        assetInstance.Init(this.gameObject);
        startNode = assetInstance.GetStartNode();
        updateNode = assetInstance.GetUpdateNode();
    }

    public void Update()
    {
        ProcessAndMoveToNextNode(updateNode);
    }

    private void ProcessAndMoveToNextNode(CodeGraphNode currentNode)
    {
        string nextNodeId = currentNode.OnProcess(assetInstance, null);
        if (!string.IsNullOrEmpty(nextNodeId))
        {
            CodeGraphNode nextNode = assetInstance.GetNode(nextNodeId);
            ProcessAndMoveToNextNode(nextNode);
        }
    }
}
