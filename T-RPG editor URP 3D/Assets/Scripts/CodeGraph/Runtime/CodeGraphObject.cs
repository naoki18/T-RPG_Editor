using System;
using UnityEngine;
using UnityEngine.Android;
public class CodeGraphObject : MonoBehaviour
{
    [SerializeField]
    private CodeGraphAsset _asset;

    // This is to modify a new asset and not the main scriptable
    private CodeGraphAsset assetInstance;
    private void OnEnable()
    {
        assetInstance = Instantiate(_asset);
        ExecuteCodeGraph();
    }

    private void ExecuteCodeGraph()
    {
        assetInstance.Init();
        CodeGraphNode startNode = assetInstance.GetStartNode();
        ProcessAndMoveToNextNode(startNode);
    }

    private void ProcessAndMoveToNextNode(CodeGraphNode currentNode)
    {
        string nextNodeId = currentNode.OnProcess(assetInstance);
        if (!string.IsNullOrEmpty(nextNodeId))
        {
            CodeGraphNode nextNode = assetInstance.GetNode(nextNodeId);
            ProcessAndMoveToNextNode(nextNode);
        }
    }
}
