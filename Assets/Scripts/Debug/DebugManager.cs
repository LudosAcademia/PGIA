using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dataText;

    private void Awake()
    {
        GameManager.Instance.GetGameData();
    }
    private void OnEnable()
    {
        GameClient.OnTokenRecieve += PrintToken;
    }

    private void OnDisable()
    {
        GameClient.OnTokenRecieve -= PrintToken;
    }

    private void Start()
    {
        //GenerateDummyData(2, 100);
    }

    public void PrintData()
    {
        GameData data = GameManager.Instance.GetGameData();
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
        dataText.text = text;
    }


    private void PrintToken(string token)
    {
        dataText.text = token;
    }


}


/*
 
    public void GenerateDummyData(int numPlay, int numTiles)
    {
        GameData gameData = new GameData();
        gameData.currentUser = new UserData();
        gameData.currentUser.name = "JohnDoe";
        //gameData.currentUser.id = 14;
        gameData.currentUser.curr_ply_index = 0;

        gameData.currentUser.playgrounds = new List<PlaygroundData>();

        for (int i = 0; i < numPlay; i++)
        {
            PlaygroundData newPlayground = new PlaygroundData();
            newPlayground.id = i;
            newPlayground.plygrd_name = "name " + i;
            newPlayground.plygrd_desc = "description " + i;
            newPlayground.plygrd_size = numTiles;
            newPlayground.tiles = new TileData[numTiles];
            newPlayground.tiles_level = new TileData[numTiles];

            for (int j = 0; j < numTiles; j++)
            {
                TileData newTile = new TileData();
                newTile.tile_index = j;
                newTile.tile_rot_y = UnityEngine.Random.Range(1, 100);
                newPlayground.tiles[i] = newTile;
                newPlayground.tiles_level[i] = newTile;
            }

            gameData.currentUser.playgrounds.Add(newPlayground);
        }

        string json = JsonUtility.ToJson(gameData.currentUser, true);
        File.WriteAllText(Application.dataPath + "/SaveData.json", json);

    }

 
 
 
 */
