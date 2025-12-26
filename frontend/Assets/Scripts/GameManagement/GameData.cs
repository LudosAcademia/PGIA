using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    //send jwt for confimation and extra json for data
    public UserData currentUser;
    public string userToken;
    public GameConfig gameConfig;
    public List<PlaygroundData> sessionPlaygrounds = new List<PlaygroundData>();
}


/*
 
    public List<UserData> userData = new List<UserData>();
    public int currentUserIndex = 0;


public enum UserType
{
    Teacher,
    Student
}


 */