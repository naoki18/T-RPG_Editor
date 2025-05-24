using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

[Serializable]
public class CodeGraphNode
{
    public const int OUTPUT = 0;
    public const int INPUT = 1;

    public enum ProcessType
    {
        None = 0,
        Loop = 1,
        Event = 2
    }

    [Serializable]
    public struct LinkedValue
    {
        public string inputValue;
        public string outputValue;
        public string inputNodeId;
        public LinkedValue(string _inputValue, string _outputValue, string _inputNodeId)
        {
            inputValue = _inputValue;
            outputValue = _outputValue;
            inputNodeId = _inputNodeId;
        }
    }

    private static Stack<string> savePoints = new Stack<string>();

    [SerializeField] private string _guid;
    [SerializeField] private Rect _position;
    public string typeName;
    public string id => _guid;
    public Rect position => _position;

    public string NodeName;

    public List<LinkedValue> linkedValues;
    public CodeGraphNode(string nodeName)
    {
        linkedValues = new List<LinkedValue>();
        NewGUID();
        NodeName = nodeName;
    }

    private void NewGUID()
    {
        _guid = Guid.NewGuid().ToString();
    }

    public void SetPosition(Rect position)
    {
        _position = position;
    }

    public string OnProcess(CodeGraphAsset graph, int outputPort, ProcessType process = ProcessType.None)
    {
        switch(process)
        {
            case ProcessType.Loop:
                savePoints.Push(id);
                break;
            case ProcessType.Event:
                EventNode eventNode = (this as EventNode);
                EventInfo eventInfo = Assembly.GetAssembly(typeof(EventNode)).GetType(eventNode.ClassName).GetEvent(NodeName);
                //Assembly.GetAssembly(typeof(EventNode)).GetType(eventNode.ClassName).GetMethod(eventNode.MethodName).
                break;
            case ProcessType.None:
            default:
                break;

        }

        CodeGraphNode node = graph.GetNextNode(_guid, outputPort);
        if (linkedValues.Count > 0)
        {
            foreach (LinkedValue linkedValue in linkedValues)
            {
                CodeGraphNode inputNode = graph.GetNode(linkedValue.inputNodeId);
                object _outputValue = this.GetType().GetField(linkedValue.outputValue).GetValue(this);
                FieldInfo inputField = inputNode.GetType().GetField(linkedValue.inputValue);

                if (inputField == null && _outputValue != null && inputNode is GenericNode genNode)
                {
                    Type inputType = genNode.GetType();
                    // Récupérer la liste des paramètres
                    List<ParamInformation> parameters = genNode.Args;

                    // Trouver le paramètre correspondant au nom du port
                    ParamInformation param = parameters.FirstOrDefault(x => x.Name == linkedValue.inputValue);

                    // Assigner la valeur dans le champ (inputField est un FieldInfo)
                    inputField = param.GetType().GetField("Value");
                    inputField.SetValue(param, _outputValue);
                }

                else if (inputField != null && _outputValue != null)
                {
                    inputField.SetValue(inputNode, _outputValue);
                }

            }
        }
        if (node != null)
        {
            return node.id;
        }

        if(savePoints.Count > 0) return savePoints.Pop();

        return string.Empty;
    }

    public virtual string OnProcess(CodeGraphAsset graph)
    {
        return OnProcess(graph, 0, ProcessType.None);
    }
}
