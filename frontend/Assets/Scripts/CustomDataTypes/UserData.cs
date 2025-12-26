using System;
using System.Collections.Generic;

[Serializable]
public class UserData 
{
    public string name;
    //current playground index:
    public int curr_ply_index = 0;

    public List<PlaygroundData> playgrounds = new List<PlaygroundData>();
    
    public UserData() { }

    public UserData(int id, string name)
    {
        this.name = name;
    }
}
