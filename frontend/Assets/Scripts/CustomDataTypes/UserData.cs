using System;
using System.Collections.Generic;

[Serializable]
public class UserData
{
    public int iD;
    public UserType type;
    public string username;
    public string password;
    public string email;
    public int phone;
    
    public List<PlaygroundData> playgroundDatas = new List<PlaygroundData>();
    public int currentPlaygroundIndex = 0;

    public UserData() { }

    public UserData(int iD, UserType type, string name, string password, string email)
    {
        this.iD = iD;
        this.type = type;
        this.username = name;
        this.password = password;
        this.email = email;
    }
}
