using System;
using System.Collections;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class GameClient : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI userInfo;
    [SerializeField] private GameObject startButton; 

    string phpApiUrl = "";
    string pythonApiUrl = "";


    public static event Action<string> OnTokenRecieve;

    public void Start()
    {
        StartCoroutine(GetGameConfig());
    }

    //read the JSON file inside the game folder and write it in the config class for later use
    IEnumerator GetGameConfig()
    {
        //string url = Application.absoluteURL;
        //string basePath = url.Substring(0, url.LastIndexOf("/"));
        //string configUrl = basePath + "/game_config.json";

        //string configUrl = Application.streamingAssetsPath + "/game_config.json";
        string configUrl = new Uri(new Uri(Application.absoluteURL), "game_config.json").ToString();
        Debug.Log("Get Config: " + configUrl);
        UnityWebRequest request = UnityWebRequest.Get(configUrl);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
        }
        else
        {
            string config = request.downloadHandler.text;
            //GameManager.Instance.GetGameData().userToken = token;
            //OnTokenRecieve?.Invoke(token);

            GameConfig newConfig = JsonUtility.FromJson<GameConfig>(config);
            GameManager.Instance.GetGameData().gameConfig = newConfig;
            //Debug.Log(phpApiUrl);
            phpApiUrl = GameManager.Instance.GetGameData().gameConfig.apiBaseUrl + GameManager.Instance.GetGameData().gameConfig.authPhpEndpoint;
            pythonApiUrl = GameManager.Instance.GetGameData().gameConfig.apiBaseUrl + GameManager.Instance.GetGameData().gameConfig.getDataPythonEndpoint;
            StartCoroutine(GetToken(phpApiUrl));
        }
    }

    //Get Raw JWT from Auth Endpoint //Default: game_connect.php
    IEnumerator GetToken(string url)
    {
        Debug.Log("Get JWT: " + url);
        Debug.Log("php url: " + phpApiUrl);

        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
        }
        else
        {
            string token = request.downloadHandler.text;
            //GameManager.Instance.GetGameData().userToken = token;
            //OnTokenRecieve?.Invoke(token);
            print(token);
            DecodeJWT(token);
        }

    }

    //Decodes the JWT into readable JSON and inserts that JSON into it respective class

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
        //string url = GameManager.Instance.GetGameData().gameConfig.apiBaseUrl + GameManager.Instance.GetGameData().gameConfig.getDataPythonEndpoint;
        //string url = "http://localhost:5000/get_data";
        Debug.Log("Get UserData: " + pythonApiUrl);
        UnityWebRequest request = new UnityWebRequest(pythonApiUrl, "POST");
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
            UserData userData = new UserData();
            userData = new UserData();
            userData = JsonUtility.FromJson<UserData>(jsonData);
            GameManager.Instance.GameData.userToken = jwt;
            GameManager.Instance.GameData.currentUser = userData;
            PrintUserData(GameManager.Instance.GameData);
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }

    }


    private void PrintData(object data)
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
        userInfo.text = text;

    }



    private void PrintUserData(GameData data)
    {
        userInfo.text = "Welcome Back " + data.currentUser.username;
    }

    public void GoCreationScene()
    {
        GameManager.Instance.GoToLevel("CreationScene");
    }


}



/*
 * 
 *     private void DecodeJWT(string jwt)
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
            Debug.Log(data);

            GameData gameData = JsonUtility.FromJson<GameData>(data);
            ParseJsonData(jwt, data);
        }
    }


    private void ParseJsonData(string jwt, string json)
    {

        GameData gameData = new GameData();
        gameData.userToken = jwt;
        gameData.currentUser = new UserData();
        gameData.currentUser = JsonUtility.FromJson<UserData>(json);

        string payload = JsonUtility.ToJson(gameData.currentUser.playgrounds, true);
        PrintUserData(gameData);
        StartCoroutine(HandlePayload(jwt, payload, pythonApiUrl));
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
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }

    }


 * 
 * 
 * http://localhost:8000/game_connect.php
 * 
 *     //string pythonApiUrl = "http://localhost:5000/send_data";

 * 
 * 
 * [Serializable]
public class DummyDataTiles
{
    public int playground_id;
    public int[] tile_index;
    public string[] tile_contain;
}

[Serializable]
public class DummyDataPlayground
{
    public string p_name = "";
    public string p_desc = "";
}

[Serializable]
public class DummyDataUser
{
    public string user_id;
    public string username;
}

    public void TestGetToken() { StartCoroutine(GetToken()); }

    public void TestDataSetup()
    {
        List<UserData> userData = new List<UserData>();

        userData.Add(new UserData(0, UserType.Teacher, "Steve", "Steve123", "steve12@gmail.com"));
        userData.Add(new UserData(1, UserType.Student, "Larry", "Larry123", "larry12@gmail.com"));
        userData.Add(new UserData(2, UserType.Student, "Sam", "Sam123", "sam12@gmail.com"));
        userData.Add(new UserData(3, UserType.Student, "Jenny", "Jenny123", "jenny12@gmail.com"));
        GameManager.Instance.GetGameData().userData = userData;
    }

    public void SendDumyData(int i)
    {
        
        if (i == 0)
        {
            DummyDataPlayground data = CreateDummyPlayground("Gamma_Playground", 
                "A creative space to try out database queries and optimization techniques");
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(Application.dataPath + "/JsonData/DummyDataTiles.json", json);
            Debug.Log("Game Saved");
            StartCoroutine(SendData(data));
        }
        else
        {
            DummyDataTiles data = CreateDummyTiles(100);
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(Application.dataPath + "/JsonData/DummyDataTiles.json", json);
            Debug.Log("Game Saved");
            StartCoroutine(SendData(data));
        }

       
    }

    private DummyDataPlayground CreateDummyPlayground(string name, string desc)
    {
        DummyDataPlayground data = new DummyDataPlayground();
        data.p_name = name;
        data.p_desc = desc;
        return data;
    }

    private DummyDataTiles CreateDummyTiles(int size)
    {
        DummyDataTiles data = new DummyDataTiles();
        data.playground_id = 17;
        data.tile_contain = new string[size];
        data.tile_index = new int[size];
        for (int i = 0; i < size; i++)
        {
            data.tile_contain[i] = "empty";
            data.tile_index[i] = i;
        }

        return data;
    }

    IEnumerator GetUserData()
    {
        UnityWebRequest request =
            UnityWebRequest.Get("http://localhost:5000/get_user_id");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
        }
        else
        {
            string json = request.downloadHandler.text;
            Debug.Log(json);
        }
    }

    IEnumerator GetPlaygroundData()
    {
        UnityWebRequest request =
            UnityWebRequest.Get("http://localhost:5000/get_plygrd_id");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
        }
        else
        {
            string json = request.downloadHandler.text;
            Debug.Log(json);
        }
    }

    IEnumerator SendData(DummyDataTiles data)
    {
        string json = File.ReadAllText(Application.dataPath + "/JsonData/DummyDataTiles.json");

        
        UnityWebRequest request = new UnityWebRequest(pythonApiUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

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

    IEnumerator SendData(DummyDataPlayground data)
    {
        string json = File.ReadAllText(Application.dataPath + "/JsonData/DummyDataTiles.json");


        UnityWebRequest request = new UnityWebRequest(pythonApiUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

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

 * 
 * 
 * 
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
