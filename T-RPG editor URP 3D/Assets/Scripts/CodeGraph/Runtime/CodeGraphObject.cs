using System.Linq;
using UnityEngine;
public class CodeGraphObject : MonoBehaviour
{
    [SerializeField]
    private CodeGraphAsset _asset;

    // This is to modify a new asset and not the main scriptable
    private CodeGraphAsset assetInstance;

    private CodeGraphNode startNode;
    private CodeGraphNode updateNode;

    public CodeGraphAsset Asset
    {
        get => _asset;
        set
        {
            if(_asset == null)
            {
                _asset = value;
            }
        }
    }
    private void Start()
    {
        assetInstance = Instantiate(_asset);
        for (int i = 0; i < _asset.Nodes.Count; i++)
        {
            if(_asset.Nodes[i] is GenericNode genNode)
            {
                (assetInstance.Nodes[i] as GenericNode).MethodName = genNode.MethodName;
                (assetInstance.Nodes[i] as GenericNode).ClassName = genNode.ClassName;
            }
        }
        InitGraph();
        if(startNode != null) ProcessAndMoveToNextNode(startNode);
    }

    private void InitGraph()
    {
        assetInstance.Init(this.gameObject);
        startNode = assetInstance.GetStartNode();
        updateNode = assetInstance.GetUpdateNode();
    }

    public void Update()
    {
        if(updateNode != null) ProcessAndMoveToNextNode(updateNode);
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
