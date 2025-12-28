using System;
using System.Collections.Generic;

[Serializable]
public class UserData 
{   
    //user name:
    public string name;
    //current playground index:
    public int curr_ply_index = 0;
    //list of playgrounds user have:
    public List<PlaygroundData> playgrounds = new();
    
    public UserData() { }

    public UserData(string name)
    {
        this.name = name;
    }
}
