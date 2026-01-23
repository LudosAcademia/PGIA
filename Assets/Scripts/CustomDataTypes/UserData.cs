using System;
using System.Collections.Generic;

[Serializable]
public class UserData
{
    //user name:
    public string username;
    //current playground index:
    public int curr_ply_index = -1;
    //list of playgrounds user have:
    public List<PlaygroundData> playgrounds = new();

    public UserData() { }

    public UserData(string name)
    {
        this.username = name;
    }

    public string PrintUserData()
    {
        string playgroundText = string.Empty;
        foreach (var item in playgrounds)
        {
            playgroundText += item.PrintPlaygroundData();
        }

        return "username: " + username + "\n current playground index: " + curr_ply_index + "Playgrounds: " + playgroundText;
    }

}
