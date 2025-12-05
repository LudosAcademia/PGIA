using System.Collections.Generic;
using UnityEngine;

public class DatabaseClient : MonoBehaviour
{
    //string apiUrl = "http://localhost:3000/players";

    public void Start()
    {
        GameManager.Instance.GetGameData();
        TestDataSetup();
    }


    public void TestDataSetup()
    {
        List<UserData> userData = new List<UserData>();

        userData.Add(new UserData(0, UserType.Teacher, "Steve", "Steve123", "steve12@gmail.com"));
        userData.Add(new UserData(1, UserType.Student, "Larry", "Larry123", "larry12@gmail.com"));
        userData.Add(new UserData(2, UserType.Student, "Sam", "Sam123", "sam12@gmail.com"));
        userData.Add(new UserData(3, UserType.Student, "Jenny", "Jenny123", "jenny12@gmail.com"));
        GameManager.Instance.GetGameData().userData = userData;
    }

}

/*
     IEnumerator GetData()
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Players: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }
 */
