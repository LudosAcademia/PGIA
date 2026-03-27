using GameEnums;
using System.Collections.Generic;

public class NodeField
{
    public string name;
    public FieldInput fieldInputType;
    public List<Node> nodeRefs;
    public bool isAssigned = false;
    public List<int> nodeConnectRefs = new List<int>();
}
