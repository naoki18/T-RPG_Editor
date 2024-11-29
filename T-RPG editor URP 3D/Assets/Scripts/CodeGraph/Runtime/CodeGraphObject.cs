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
