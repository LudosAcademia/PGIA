using System;
using UnityEngine;

public class ModifyPlayground : MonoBehaviour
{
    public static event Action<PlaygroundData> OnPlaygroundModified;

    public void ModifySelectedPlayground(int index, PlaygroundData playground)
    {

    }

    public TileData ModifyPlaygroundTile(int tile_index, string tile_contain)
    {

        return null;    
    }

}


/*
//tile data
    public int tile_index;
    public string tile_contain = "empty";

    public int tile_pos_x = 0;
    public int tile_pos_y = 0;
    public int tile_pos_z = 0;

    public int tile_rot_y = 0;

//playground data
    public bool delete = false;
    public int id;
    public string plygrd_name;
    public string plygrd_desc;
    public List<TileData> tiles = new();
    public int plygrd_size;

 
 */