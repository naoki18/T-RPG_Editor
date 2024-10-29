using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct CodeGraphConnection
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
}
[Serializable]
public struct CodeGraphConnectionPort
{
    public string nodeId;
    public int portIndex;

    public CodeGraphConnectionPort(string id, int index)
    {
        nodeId = id;
        portIndex = index;
    }
}

