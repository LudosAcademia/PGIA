using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class DatabaseClient : MonoBehaviour
{
    string apiUrl = "http://localhost:3000/players";

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



}
