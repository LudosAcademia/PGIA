using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ServerClient : MonoBehaviour
{
    public static event Action OnServerWait;
    public static event Action<bool> PlaygroundSaved;
    public static event Action<bool> PlaygroundUpdated;
    public static event Action<bool> PlaygroundDeleted;


    private void OnEnable()
    {
        CreatePlayground.OnPlaygroundCreated += SendNewPlayground;
        PlaygroundManager.OnEndPlaygroundDelete += SendData;
        PlaygroundManager.OnEndPlaygroundEdit += SendData;
    }

    private void OnDisable()
    {
        CreatePlayground.OnPlaygroundCreated -= SendNewPlayground;
        PlaygroundManager.OnEndPlaygroundDelete -= SendData;
        PlaygroundManager.OnEndPlaygroundEdit -= SendData;
    }

    private void SendNewPlayground(PlaygroundData newPlayground)
    {
        if (GameManager.Instance.debug)
        {
            GameManager.Instance.GameData.currentUser.playgrounds.Add(newPlayground);
            PlaygroundSaved?.Invoke(true);
            return;
        }

        //VERY IMPORTANT ASSIGNMENT:---------------------------
        newPlayground.layer_count = newPlayground.tiles_arrays.Count;
        //-----------------------------------------------------
        string url = GameManager.Instance.GetGameData().gameConfig.apiBaseUrl + GameManager.Instance.GetGameData().gameConfig.sendDataPythonEndpoint;
        string jwt = GameManager.Instance.GetGameData().userToken;
        string payload = JsonUtility.ToJson(newPlayground, true);
        Debug.Log(payload);
        OnServerWait?.Invoke();
        StartCoroutine(HandlePayload(jwt, payload, url, newPlayground));
    }


    private void SendData()
    {
        int index = GameManager.Instance.GameData.currentUser.curr_ply_index;
        PlaygroundData selectedPlayground = GameManager.Instance.GameData.currentUser.playgrounds[index];

        if (GameManager.Instance.debug)
        {
            if (selectedPlayground.update)
            {
                PlaygroundUpdated?.Invoke(true);
            }
            else if (selectedPlayground.delete)
            {
                PlaygroundDeleted?.Invoke(true);
            }
            return;
        }

        //VERY IMPORTANT ASSIGNMENT:---------------------------
        selectedPlayground.layer_count = selectedPlayground.tiles_arrays.Count;
        //-----------------------------------------------------
        string url = GameManager.Instance.GetGameData().gameConfig.apiBaseUrl + GameManager.Instance.GetGameData().gameConfig.sendDataPythonEndpoint;
        string jwt = GameManager.Instance.GetGameData().userToken;
        string payload = JsonUtility.ToJson(selectedPlayground, true);
        Debug.Log(payload);
        OnServerWait?.Invoke();
        StartCoroutine(HandlePayload(jwt, payload, url, selectedPlayground));
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

            if (playground.update)
            {
                int index = GameManager.Instance.GameData.currentUser.curr_ply_index;
                GameManager.Instance.GameData.currentUser.playgrounds[index].update = false;
                PlaygroundUpdated?.Invoke(true);
            }
            else if (playground.delete)
            {
                int index = GameManager.Instance.GameData.currentUser.curr_ply_index;
                GameManager.Instance.GameData.currentUser.playgrounds.RemoveAt(index);
                GameManager.Instance.GameData.currentUser.curr_ply_index = -1;
                //Debug.Log(GameManager.Instance.GameData.currentUser.PrintUserData());
                PlaygroundDeleted?.Invoke(true);
            }
            else
            {
                string data = request.downloadHandler.text;
                GameManager.Instance.GameData.currentUser.playgrounds.Add(playground);
                int lastIndex = GameManager.Instance.GameData.currentUser.playgrounds.Count - 1;
                GameManager.Instance.GameData.currentUser.playgrounds[lastIndex].id = JsonUtility.FromJson<TempPlaygroundId>(data).plygrd_id;

                foreach (var item in GameManager.Instance.GameData.currentUser.playgrounds)
                {
                    Debug.Log("username: " + item.plygrd_name);
                    Debug.Log("guid: " + item.id);

                }

                PlaygroundSaved?.Invoke(true);
            }

        }
        else
        {
            Debug.LogError("Error: " + request.error);

            if (playground.update)
            {
                PlaygroundUpdated?.Invoke(false);
            }
            else if (playground.delete)
            {
                PlaygroundDeleted?.Invoke(false);
            }
            else
            {
                PlaygroundSaved?.Invoke(false);
            }

        }

    }





}

[Serializable]
public class TempPlaygroundId
{
    public int plygrd_id;
}
