using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreatePlayground : MonoBehaviour
{
    [SerializeField] private TMP_InputField playgroundName;
    [SerializeField] private TMP_InputField playgroundDesc;
    [SerializeField] private TMP_Dropdown playgroundSize;

    public static event Action<PlaygroundData> OnPlaygroundCreated;

    public void CreateNewPlayground()
    {
        string name = playgroundName.text;
        string desc = playgroundDesc.text;
        int size = 6;

        if (playgroundSize.value == 0)
        {
            size = 6;
        }
        else if (playgroundSize.value == 1)
        {
            size = 8;
        }
        else if(playgroundSize.value == 2)
        {
            size = 10;
        }

        PlaygroundData newPlayground = new PlaygroundData();
        List<PlaygroundData> allPlaygroundData = GameManager.Instance.GetGameData().currentUser.playgrounds;

        newPlayground.id = -1;
        newPlayground.plygrd_name = name;
        newPlayground.plygrd_desc = desc;
        newPlayground.plygrd_size = size * size;
        newPlayground.tilesArray = new();
        TileDataArray tileDataArray = new TileDataArray();
        tileDataArray.tiles = new TileData[newPlayground.plygrd_size];
        tileDataArray.layer = "Base";

        for (int i = 0; i < newPlayground.plygrd_size; i++)
        {
            TileData newTile = new TileData();
            newTile.tile_index = i;
            newTile.tile_contain_id = -1;
            newTile.tile_rot_y = 0;
            tileDataArray.tiles[i] = newTile;
        }
        newPlayground.tilesArray.Add(tileDataArray);

        //DebugPlaygroundData(newPlayground);
        OnPlaygroundCreated?.Invoke(newPlayground);

    }

    private void DebugPlaygroundData(PlaygroundData data)
    {
        int size = data.plygrd_size;
        //TileData[] tilesBase = data.tiles;

        for (int i = 0; i < size; i++)
        {
            //Debug.Log("The Tile Base at " + i + " Contain id:  " + tilesBase[i].tile_contain_id);
        }
    }


}




/*
 playground (id = -1,name,desc,tiles)
 
 */