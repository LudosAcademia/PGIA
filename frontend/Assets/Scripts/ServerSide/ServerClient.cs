using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ServerClient : MonoBehaviour
{
    private void OnEnable()
    {
        CreatePlayground.OnPlaygroundCreated += SendData;
    }

    private void OnDisable()
    {
        CreatePlayground.OnPlaygroundCreated -= SendData;
    }

    private void SendData(PlaygroundData playgrounds)
    {
        string url = GameManager.Instance.GetGameData().gameConfig.apiBaseUrl + GameManager.Instance.GetGameData().gameConfig.authPythonEndpoint;
        string jwt = GameManager.Instance.GetGameData().userToken;
        string payload = JsonUtility.ToJson(playgrounds, true);
        StartCoroutine(HandlePayload(jwt, payload, url));        
    }


    IEnumerator HandlePayload(string jwt, string payload, string url)
    {
        //string url = GameManager.Instance.GetGameData().gameConfig.apiBaseUrl + GameManager.Instance.GetGameData().gameConfig.authPythonEndpoint;
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(payload);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", "Bearer " + jwt);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Recieved Payload: " + request.downloadHandler.text);
            string data = request.downloadHandler.text;
            GameData gameData = new GameData();
            gameData.currentUser.playgrounds = JsonUtility.FromJson<List<PlaygroundData>>(data);
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }

    }



}
