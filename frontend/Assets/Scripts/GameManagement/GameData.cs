using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public List<UserData> userData = new List<UserData>();
    public UserData currentUser;
}


public enum UserType
{
    Teacher,
    Student
}