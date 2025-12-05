using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public List<UserData> userData = new List<UserData>();
    public int currentUserIndex = 0;
    public UserData currentUser;
}


public enum UserType
{
    Teacher,
    Student
}