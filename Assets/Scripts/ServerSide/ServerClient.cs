using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ServerClient : MonoBehaviour
{
    public static event Action<bool> PlaygroundSaved;

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
        string url = GameManager.Instance.GetGameData().gameConfig.apiBaseUrl + GameManager.Instance.GetGameData().gameConfig.sendDataPythonEndpoint;
        string jwt = GameManager.Instance.GetGameData().userToken;
        string payload = JsonUtility.ToJson(playgrounds, true);
        Debug.Log(payload);
        StartCoroutine(HandlePayload(jwt, payload, url, playgrounds));
    }


    IEnumerator HandlePayload(string jwt, string payload, string url, PlaygroundData playground)
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
            GameManager.Instance.GameData.currentUser.playgrounds.Add(playground);
            //PlaygroundData idHolder = new();
            //idHolder = JsonUtility.FromJson<PlaygroundData>(data);

            int lastIndex = GameManager.Instance.GameData.currentUser.playgrounds.Count - 1;
            GameManager.Instance.GameData.currentUser.playgrounds[lastIndex].id = JsonUtility.FromJson<TempPlaygroundId>(data).plygrd_id;

            foreach (var item in GameManager.Instance.GameData.currentUser.playgrounds)
            {
                Debug.Log("name: " + item.plygrd_name);
                Debug.Log("id: " + item.id);

            }
            //idHolder = null;
            PlaygroundSaved?.Invoke(true);
        }
        else
        {
            Debug.LogError("Error: " + request.error);
            PlaygroundSaved?.Invoke(false);
        }

    }

}

[Serializable]
public class TempPlaygroundId
{
    public int plygrd_id;
}
