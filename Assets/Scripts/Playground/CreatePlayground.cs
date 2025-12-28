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
        int size = 25;
        if (playgroundSize.value == 0)
        {
            size = 25;
        }
        else
        {
            size = 100;
        }

        PlaygroundData newPlayground = new PlaygroundData();
        List<PlaygroundData> allPlaygroundData = GameManager.Instance.GetGameData().currentUser.playgrounds;

        newPlayground.id = -1;
        newPlayground.plygrd_name = name;
        newPlayground.plygrd_desc = desc;
        newPlayground.tiles = new List<TileData>();
        for (int i = 0; i < size; i++)
        {
            TileData newTile = new TileData();
            newTile.tile_index = i;
            newPlayground.tiles.Add(newTile);
        }
        newPlayground.plygrd_size = size;
        OnPlaygroundCreated?.Invoke(newPlayground);

    }

}


/*
 playground (id = -1,name,desc,tiles)
 
 */