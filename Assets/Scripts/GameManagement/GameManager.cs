using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    [SerializeField] public bool debug = false;
    private GameData gameData;

    public static GameManager Instance { get; set; }
    public GameData GameData { get => gameData; set => gameData = value; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        gameData ??= new GameData();
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
 * 
 *   get
        {
            if (BaseInstant == null)
            {
                //_instance = FindFirstObjectByType<GameManager>();
                GameObject newObj = new GameObject();
                newObj.name = "Game Manager";
                BaseInstant = newObj.AddComponent<GameManager>();
                BaseInstant.gameData = new GameData();
            }

            return BaseInstant;
        }
        set
        {
            BaseInstant = value;
        }

    }

 * 
 * 
 *     public static GameManager Instance
    {
      
    }
 * 
 * 
    //Save Game by calling this function
    public void SaveGame()
    {
        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(Application.dataPath + "/SaveData.json", json);
        Debug.LogError("Game Save currently not implemented!");
    }

    //Load Game by calling this function
    public void LoadGame()
    {
        string json = File.ReadAllText(Application.dataPath + "/SaveData.json");
        gameData = JsonUtility.FromJson<GameData>(json);
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