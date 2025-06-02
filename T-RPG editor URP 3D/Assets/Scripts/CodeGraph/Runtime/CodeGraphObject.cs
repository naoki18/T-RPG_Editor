using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEditor;
using UnityEngine;
public class CodeGraphObject : MonoBehaviour
{
    [SerializeField]
    private CodeGraphAsset _asset;

    // This is to modify a new asset and not the main scriptable
    private CodeGraphAsset assetInstance;

    private CodeGraphNode startNode;
    private CodeGraphNode updateNode;

    private List<string> eventsNode = new List<string>();
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
        if (currentNode is EventNode eventNode && eventNode?.Component != null && !eventsNode.Contains(currentNode.id))
        {
            eventsNode.Add(currentNode.id);
            Type classType = eventNode.Component.GetType();
            EventInfo eventInfo = classType.GetEvent(eventNode.NodeName);
            if (eventInfo != null)
            {
                Delegate handlerDelegate = CreateUniversalDelegate(eventInfo, currentNode.id);
                eventInfo.AddEventHandler(eventNode.Component, handlerDelegate);
            }
            else Debug.LogError($"Event '{eventNode.NodeName}' introuvable dans le type {classType.Name}.");
        }

        string nextNodeId = currentNode.OnProcess(assetInstance);

        if (!string.IsNullOrEmpty(nextNodeId))
        {
            CodeGraphNode nextNode = assetInstance.GetNode(nextNodeId);
            ProcessAndMoveToNextNode(nextNode);
        }
    }

    private void ProcessEventAndMoveToNextNode(CodeGraphNode currentNode)
    {
        string nextNodeId = currentNode.OnProcess(assetInstance, EventNode.eventPortIndex);

        if (!string.IsNullOrEmpty(nextNodeId))
        {
            CodeGraphNode nextNode = assetInstance.GetNode(nextNodeId);
            ProcessAndMoveToNextNode(nextNode);
        }
    }
    // It's painful to do these things and I really want to re think all the architecture to avoid that. Wanna cry right now
    public Delegate CreateUniversalDelegate(EventInfo eventInfo, string id)
    {
        Type eventHandlerType = eventInfo.EventHandlerType;
        MethodInfo invokeMethod = eventHandlerType.GetMethod("Invoke");
        ParameterInfo[] parameters = invokeMethod.GetParameters();

        ConstantExpression instance = Expression.Constant(this);
        MethodInfo method = typeof(CodeGraphObject).GetMethod("OnEventTriggered", BindingFlags.Instance | BindingFlags.NonPublic);

        ParameterExpression[] paramExprs = parameters.Select(p => Expression.Parameter(p.ParameterType, p.Name)).ToArray();
        List<UnaryExpression> castToObject = paramExprs.Select(p => Expression.Convert(p, typeof(object))).ToList();
        NewArrayExpression arrayExpr = Expression.NewArrayInit(typeof(object), castToObject);

        MethodCallExpression body = Expression.Call(instance, method, arrayExpr, Expression.Constant(id));
        LambdaExpression lambda = Expression.Lambda(eventHandlerType, body, paramExprs);

        return lambda.Compile();
    }

    private void OnEventTriggered(object[] args, string id)
    {
        Debug.Log($"Événement déclenché pour avec {args.Length} arguments.");
        EventNode node = assetInstance.GetNode(id) as EventNode;

        for(int i = 0; i < args.Length; ++i)
        {
            node.Args[i].Value = args[i];
        }
        if (node != null)
        {
            Debug.Log($"Node {node.NodeName} was triggered.");
            ProcessEventAndMoveToNextNode(node);
        }
    }
}
