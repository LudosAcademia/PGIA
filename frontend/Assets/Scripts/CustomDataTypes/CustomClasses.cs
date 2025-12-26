using System;

[Serializable]
public class CustomClasses
{

}

[Serializable]
public class TempUser
{
    public string username;
    public string password;
}

[Serializable]
public class TempUserData
{
    public int id;
    public string name;
}

public class GameConfig
{
    public string apiBaseUrl;
    public string authPhpEndpoint;
    public string authPythonEndpoint;
}