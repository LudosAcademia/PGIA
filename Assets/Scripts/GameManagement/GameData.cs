using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    //send jwt for confimation and extra json for data
    public UserData currentUser = new();
    public string userToken;
    public GameConfig gameConfig = new();
    public List<PlaygroundData> sessionPlaygrounds = new();
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