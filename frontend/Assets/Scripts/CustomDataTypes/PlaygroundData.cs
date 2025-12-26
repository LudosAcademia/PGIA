using System;
using System.Collections.Generic;

[Serializable]
public class PlaygroundData
{
    public bool delete = false;
    public int id;
    public string plygrd_name;
    public string plygrd_desc;
    public List<TileData> tiles = new List<TileData>();
    public int plygrd_size;

}



