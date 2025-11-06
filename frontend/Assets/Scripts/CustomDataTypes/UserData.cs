using System;

[Serializable]
public class UserData
{
    public UserType type;
    public string username;
    public string password;
    public string email;

    public UserData() { }

    public UserData(UserType type, string name, string password, string email)
    {
        this.type = type;
        this.username = name;   
        this.password = password;
        this.email = email;
    }
}
