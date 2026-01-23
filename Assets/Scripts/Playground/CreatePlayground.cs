using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreatePlayground : MonoBehaviour
{
    public static event Action<PlaygroundData> OnPlaygroundCreated;
    string playgroundName;
    string playgroundDesc;
    int playgroundSize;

    public void SetPlaygroundParameters(string name, string desc, int SizeIndex)
    {
        playgroundName = name;
        playgroundDesc = desc;

        if (SizeIndex == 0)
        {
            playgroundSize = 6;
        }
        else if (SizeIndex == 1)
        {
            playgroundSize = 8;
        }
        else if (SizeIndex == 2)
        {
            playgroundSize = 10;
        }
    }

    private void OnEnable()
    {
        PlaygroundUI.OnPlaygroundInputValueChange += SetPlaygroundParameters;
    }

    private void OnDisable()
    {
        PlaygroundUI.OnPlaygroundInputValueChange -= SetPlaygroundParameters;
    }


    public void CreateNewPlayground()
    {
        PlaygroundData newPlayground = new PlaygroundData();
        List<PlaygroundData> allPlaygroundData = GameManager.Instance.GetGameData().currentUser.playgrounds;

        newPlayground.id = -1;
        newPlayground.plygrd_name = playgroundName;
        newPlayground.plygrd_desc = playgroundDesc;
        newPlayground.plygrd_size = playgroundSize * playgroundSize;
        newPlayground.tiles_arrays = new();
        TileDataArray tileDataArray = new TileDataArray();
        tileDataArray.tiles = new TileData[newPlayground.plygrd_size];
        tileDataArray.tile_layer = "Base";

        for (int i = 0; i < newPlayground.plygrd_size; i++)
        {
            TileData newTile = new TileData();
            newTile.tile_index = i;
            newTile.tile_contain_id = -1;
            newTile.tile_rot_y = 0;
            tileDataArray.tiles[i] = newTile;
        }
        newPlayground.tiles_arrays.Add(tileDataArray);

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