using System;
using System.Collections.Generic;

public class EventData
{
    public string name;
    public Guid id;
    public List<AvaItemPreBuild> inputObjectRefs = new();
    public AvaItemPreBuild outputObjectRef;
    public AvaItemPreBuild actorObjectRef;
}


