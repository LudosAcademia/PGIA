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
    public List<TileDataArray> tilesArray;

    public int plygrd_size;

}



