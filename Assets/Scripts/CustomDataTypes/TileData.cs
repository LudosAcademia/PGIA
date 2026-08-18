using System;

//For writing data in msql database
[Serializable]
public class TileData
{
    public int tile_index = -1;
    public int tile_contain_id = 0;
    public int tile_rot_y = 0;
    public string tile_guid = string.Empty;
    public bool tile_is_logic = false;

    public TileData()
    {
    }

    public TileData(int tile_index, int tile_contain_id, int tile_rot_y)
    {
        this.tile_index = tile_index;
        this.tile_contain_id = tile_contain_id;
        this.tile_rot_y = tile_rot_y;
    }

    public string PrintTileData()
    {
        return "tile index: " + this.tile_index
            + "\n tile contain: " + this.tile_contain_id
            + "\n tile rot y: " + this.tile_rot_y;
    }

}

[Serializable]
public class TileDataArray
{
    public string tile_layer;
    public TileData[] tiles;

    public string PrintTileDataArray()
    {
        string layerText = "tile_layer: " + this.tile_layer;
        string tilesText = "";
        foreach (var item in this.tiles)
        {
            tilesText += item.PrintTileData();
        }

        return layerText + tilesText;
    }

}


/*

    public int tile_index;
    public string tile_contain = "empty";

    public int tile_pos_x = 0;
    public int tile_pos_y = 0;
    public int tile_pos_z = 0;

    public int tile_rot_y = 0;

 
 */