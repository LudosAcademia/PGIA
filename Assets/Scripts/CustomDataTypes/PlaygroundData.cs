using System;
using System.Collections.Generic;

[Serializable]
public class PlaygroundData
{
    public bool delete = false;
    public bool update = false;
    public int id;
    public string plygrd_name;
    public string plygrd_desc;
    public List<TileDataArray> tiles_arrays;
    public int plygrd_size;
    public int layer_count;


    public string PrintPlaygroundData()
    {
        string tileArrayText = string.Empty;
        string fieldText = "delete: " + this.delete +
            "\n update: " + this.update + "\n id: " + this.id + "\n plygrd_name: " + this.plygrd_name +
            "\n plygrd_desc: " + this.plygrd_desc + "\n plygrd_size: " + this.plygrd_size + "\n layer_count: " + this.layer_count;

        foreach (var item in this.tiles_arrays)
        {
            tileArrayText += item.PrintTileDataArray();
        }
        string totalText = fieldText + " Tiles Arrays: \n" + tileArrayText;

        return totalText;
    }

}



