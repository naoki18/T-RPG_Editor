using System;

[Serializable]
public struct CodeGraphConnection : IEquatable<CodeGraphConnection>
{
    public CodeGraphConnectionPort inputPort;
    public CodeGraphConnectionPort outputPort;

    public CodeGraphConnection(CodeGraphConnectionPort input, CodeGraphConnectionPort output)
    {
        this.inputPort = input;
        this.outputPort = output;
    }

    public CodeGraphConnection(string inputPortId, int inputIndex, string outputPortId, int outputId)
    {
        inputPort = new CodeGraphConnectionPort(inputPortId, inputIndex);
        outputPort = new CodeGraphConnectionPort(outputPortId, outputId);
    }

    public bool Equals(CodeGraphConnection other)
    {
        return inputPort.Equals(other.inputPort) && outputPort.Equals(other.outputPort);
    }
}
[Serializable]
public struct CodeGraphConnectionPort : IEquatable<CodeGraphConnectionPort>
{
    public string nodeId;
    public int portIndex;

    public CodeGraphConnectionPort(string id, int index)
    {
        nodeId = id;
        portIndex = index;
    }

    public bool Equals(CodeGraphConnectionPort other)
    {
        return nodeId == other.nodeId && portIndex == other.portIndex;
    }
}

