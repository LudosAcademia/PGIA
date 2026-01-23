using System;
using System.Collections;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class AuthenticateClient : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameInfo;
    [SerializeField] private TMP_InputField passwordInfo;
    [SerializeField] private TextMeshProUGUI userDetailText;
    [SerializeField] private GameObject startButton;

    private void Start()
    {
        GameManager.Instance.GameData.gameConfig.apiBaseUrl = "http://localhost";
        GameManager.Instance.GameData.gameConfig.authPhpEndpoint = ":8000/src/game_client_login.php";
        GameManager.Instance.GameData.gameConfig.getDataPythonEndpoint = ":5000/gameserver/get_data";
        GameManager.Instance.GameData.gameConfig.sendDataPythonEndpoint = ":5000/gameserver/send_data";

        // / gameserver / get_data
    }

    public void Login()
    {
        string username = usernameInfo.text;
        string password = passwordInfo.text;

        StartCoroutine(AuthClient(username, password));
    }

    IEnumerator AuthClient(string username, string password)
    {
        TempUser user = new TempUser();
        user.username = username;
        user.password = password;
        string json = JsonUtility.ToJson(user, true);
        string token = "";
        Debug.Log(json);

        string url = GameManager.Instance.GameData.gameConfig.apiBaseUrl + GameManager.Instance.GameData.gameConfig.authPhpEndpoint;
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Players: " + request.downloadHandler.text);
            token = request.downloadHandler.text;
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }


        DecodeJWT(token);
        //Debug.Log(token);
    }


    private void DecodeJWT(string jwt)
    {
        string[] splitToken = jwt.Split(".");

        if (splitToken.Length == 3)
        {
            string payload = splitToken[1];
            int reminder = splitToken[1].Length % 4;
            if (reminder > 0)
            {
                int missingChars = 4 - reminder;
                payload += new string('=', missingChars);
            }

            byte[] bytes = System.Convert.FromBase64String(payload);
            string data = System.Text.UTF8Encoding.UTF8.GetString(bytes);
            //Debug.Log(data);
            StartCoroutine(RecieveUserData(jwt, data));
        }
    }


    IEnumerator RecieveUserData(string jwt, string payload)
    {
        string url = GameManager.Instance.GetGameData().gameConfig.apiBaseUrl + GameManager.Instance.GetGameData().gameConfig.getDataPythonEndpoint;
        //string url = "http://localhost:5000/get_data";
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(payload);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", "Bearer " + jwt);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            startButton.SetActive(true);
            string jsonData = request.downloadHandler.text;
            Debug.Log("Recieved Payload: " + jsonData);
            //TestGameManager();
            //PrintGameConfig();
            UserData userData = JsonUtility.FromJson<UserData>(jsonData);
            GameManager.Instance.GameData.userToken = jwt;
            GameManager.Instance.GameData.currentUser = userData;
            Debug.Log("The UserData: " + userData.PrintUserData());
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }

    }


    public void PrintData(object data)
    {
        Type type = data.GetType();
        string text = "";
        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

        Debug.Log("PrintData Pressed, Field Length: " + fields.Length);

        for (int i = 0; i < fields.Length; i++)
        {
            object value = fields[i].GetValue(data);
            Debug.Log("" + fields[i].Name + ": " + value);
            text += "" + fields[i].Name + ": " + value + "\n";
        }
        userDetailText.text = text;

    }


    private void PrintUserData(GameData data)
    {
        userDetailText.text = "User Name: " + data.currentUser.username
            + "\n Current Playground: " + data.currentUser.curr_ply_index
            + "\n Playgrounds: " + data.currentUser.playgrounds.Count
            + "\n Tiles: " + data.currentUser.playgrounds[data.currentUser.curr_ply_index].tiles_arrays;
    }


    private void TestGameManager()
    {
        userDetailText.text = "User Token: " + GameManager.Instance.GameData.userToken +
            "Username: " + GameManager.Instance.GameData.currentUser.username;

    }

    private void PrintGameConfig()
    {

        userDetailText.text = "Auth Base: " + GameManager.Instance.GameData.gameConfig.apiBaseUrl + "\n" + 
            "Auth PHP: " + GameManager.Instance.GameData.gameConfig.authPhpEndpoint + "\n" +
            "Auth Python: " + GameManager.Instance.GameData.gameConfig.sendDataPythonEndpoint;

    }

    public void GoCreationScene()
    {
        GameManager.Instance.GoToLevel("CreationScene");
    }


}

/*
 * 
 * 
 *         var gm = GameManager.Instance;

        Debug.Log(gm == null);                  // false
        Debug.Log(gm.GameData == null);         // true?
        Debug.Log(gm.GameData?.gameConfig == null); // true?


 * 
    IEnumerator SendPayload(string jwt, string payload)
    {
        UnityWebRequest request = new UnityWebRequest(pythonApiUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(payload);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", "Bearer " + jwt);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Message Sent: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }


 * 
 * 
 * 
    string phpApiUrl = "http://localhost:8000/src/game_client_login.php";
    string pythonApiUrl = "http://localhost:5000/gate_data.py"; 


    public void DebugLogin()
    {
        List<UserData> userData = GameManager.Instance.GetGameData().userData;
        UserData user = userData.Find(p => p.username == usernameInfo.text && p.password == passwordInfo.text);

        for (int i = 0; i < userData.Count; i++)
        {
            Debug.Log(userData[i].username + " " + userData[i].password);
        }

        if (user == null)
        {
            userInfo = "Password or Username incorrect";
        }
        else
        {
            userInfo = "Welcome " + user.username + " \n Position: " + user.type;
            GameManager.Instance.GetGameData().currentUser = user;

            startButton.SetActive(true);
            usernameInfo.gameObject.SetActive(false);
            passwordInfo.gameObject.SetActive(false);
        }


        userDetailText.text = userInfo;
        GameManager.Instance.GetGameData().currentUserIndex = userData.FindIndex(p => p.iD == user.iD);
        Debug.Log(userInfo);
        Debug.Log(usernameInfo.text + " " + passwordInfo.text);

    }

    public void StartGame()
    {

        GameManager.Instance.GoToLevel("CreationScene");
    }


 
 */