using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    private GameData gameData;

    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                //_instance = FindFirstObjectByType<GameManager>();
                GameObject newObj = new GameObject();
                newObj.name = "Game Manager";
                instance = newObj.AddComponent<GameManager>();
            }
            return instance;
        }
        set
        {
            instance = value;
        }

    }

    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (gameData == null)
        {

            gameData = new GameData();

        }
        


    }


    //Go next level by giving level build index
    public void NextLevel(int level)
    {
        SceneManager.LoadSceneAsync(level);
    }

    public void GoToLevel(string level)
    {
        SceneManager.LoadSceneAsync(level);
        //SaveGame();
    }

    public void GoToLevel(int level)
    {
        SceneManager.LoadSceneAsync(level);
        //SaveGame();
    }


    //For setting the game data in Game Manager from a instance of a local game data object.
    public void SetGameData(GameData newGameData)
    {
        gameData = newGameData;
    }

    //For getting the initilized game data in Game Manager
    public GameData GetGameData()
    {
        return gameData;
    }

}

/*
         if (File.Exists(Application.dataPath + "/SaveData.json"))
        {
            LoadGame();
        }
        else
        {
            SaveGame();
        }
 
    //Save Game by calling this function
    public void SaveGame()
    {
        //string json = JsonUtility.ToJson(gameData, true);
        //File.WriteAllText(Application.dataPath + "/SaveData.json", json);
        Debug.LogError("Game Save currently not implemented!");
    }

    //Load Game by calling this function
    public void LoadGame()
    {
        //string json = File.ReadAllText(Application.dataPath + "/SaveData.json");
        //gameData = JsonUtility.FromJson<GameData>(json);
        Debug.LogError("Game Load currently not implemented!");

    }

    //For reseting the game data in JSON and runtime game data.
    public void ResetGameData()
    {
        gameData = new GameData();
        SaveGame();
        LoadGame();
    }



 */