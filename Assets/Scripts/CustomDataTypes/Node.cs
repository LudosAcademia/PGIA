using System.Collections.Generic;

public abstract class Node
{
    public int id;
    public string name;
    public int inputFieldNumber;
    public int outputFieldNumber;
    public NodeField[] inputfields;
    public NodeField[] outputfields;
    public bool baseNode = false;
    public bool executeReady = false;
    public NodeValue baseValue;

    public Node() { }
    public Node(string name, int inputFieldNumber, int outputFieldNumber)
    {
        id = -1;
        this.name = name;
        this.inputFieldNumber = inputFieldNumber;
        this.outputFieldNumber = outputFieldNumber;

        inputfields = new NodeField[inputFieldNumber];
        outputfields = new NodeField[outputFieldNumber];
    }

    //    public abstract NodeValue NodeOperation(NodeValue nodeValue);

    public abstract void Operation();

    public abstract void ValueAssignment();


}


public struct NodeValue
{
    public GameEnums.NodeValueType type;
    private double number;
    private bool boolean;
    private string text;

    public double AsDouble() => number;
    public int AsInt() => (int)number;
    public float AsFloat() => (float)number;
    public bool AsBool() => boolean;
    public string AsString() => text;

    public void CastDouble(double num) => number = num;
    public void CastInt(int num) => number = num;
    public void CastFloat(float num) => number = num;
    public void CastBoolean(bool bol) => boolean = bol;
    public void CastText(string txt) => text = txt;

    public override string ToString()
    {
        return "The Number As Double: " + AsDouble() + " The Text: " + AsString() + " The Boolean " + AsBool() + " " + base.ToString();
    }

}



/*
     public static NodeValue From(int v) =>
        new() { Type = ValueType.Int, number = v };

    public static NodeValue From(bool v) =>
        new() { Type = ValueType.Bool, boolean = v };

    public static NodeValue From(string v) =>
        new() { Type = ValueType.String, text = v };
 
 */