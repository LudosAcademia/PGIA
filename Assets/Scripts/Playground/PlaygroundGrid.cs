using System.Collections.Generic;
using UnityEngine;

public class PlaygroundGrid : MonoBehaviour
{
    public Dictionary<string, GridLayer> grid;
    public List<string> gridLayerKeys;
    public List<AvaItemPreBuild> logicReadyItems;
    private int gridSize;
    

    public int GridSize { get => gridSize; set => gridSize = value; }

    public PlaygroundGrid(int size, List<string> layerKeys)
    {
        gridLayerKeys = layerKeys;
        grid = new();

        foreach (var layer in gridLayerKeys)
        {
            GridLayer gridLayer = new(size);
            GridTile[,] newGridData = new GridTile[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    newGridData[i, j] = new();
                }
            }
            gridLayer.data = newGridData;
            grid.Add(layer, gridLayer);
        }
    }

    public GridTile GetDataIndex(int index, int size, string layer)
    {
        int x = index % size;
        int z = index / size;

        return grid[layer].data[x, z];
    }

    public void SetDataIndex(GridTile tile, int index, int size, string layer)
    {
        int x = index % size;
        int z = index / size;
        grid[layer].data[x, z] = tile;
    }

    public void DeleteTile(string layer, int x, int z)
    {
        grid[layer].data[x, z] = new();
        grid[layer].visuals[x, z] = null;
    }

    public void AddLayer(string layerKey)
    {
        int size = gridSize;
        GridLayer gridLayer = new(size);
        GridTile[,] newGridData = new GridTile[size, size];
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                newGridData[i, j] = new();
            }
        }
        gridLayer.data = newGridData;
        gridLayerKeys.Add(layerKey);
        grid.Add(layerKey, gridLayer);
    }


    public void DeleteLayer(string layer)
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                if (grid[layer].visuals[i, j] != null)
                {
                    Destroy(grid[layer].visuals[i, j]);
                }
            }
        }

        grid[layer].data = null;
        grid[layer].visuals = null;
        grid.Remove(layer);
        gridLayerKeys.Remove(layer);
    }

}

public class GridLayer
{
    public GridTile[,] data;
    public GameObject[,] visuals;

    public GridLayer(int size)
    {
        data = new GridTile[size, size];
        visuals = new GameObject[size, size];
    }
}


public class GridTile
{
    public int x;
    public int z;
    public int rotY;
    public int index;
    public int containId;

    public GridTile()
    {
        this.x = 0;
        this.z = 0;
        this.rotY = 0;
        this.index = -1;
        this.containId = -1;
    }

    public GridTile(int x, int z, int index, int rotY, int containId)
    {
        this.x = x;
        this.z = z;
        this.rotY = rotY;
        this.index = index;
        this.containId = containId;
    }

    public GridTile(int x, int z, int rotY, int containId)
    {
        this.x = x;
        this.z = z;
        this.rotY = rotY;
        this.containId = containId;
    }

}


/*
 * 
 

    public int[] GetTileCord(int index, GridTile[,] grid)
    {
        int iterate = 0;
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                if (index == iterate)
                {
                    int[] cords = { i, j };
                    return cords;
                }
                iterate++;
            }
        }
        return null;
    }

   

 * 
 * 
 *         if (tile == GameEnums.TileLevel.Base)
        {
            return gridTilesBase[x, z];
        }
        else
        {
            return gridTilesLevel[x, z];
        }

 * 
 * 
        if (tile == GameEnums.TileLevel.Base)
        {
            gridTilesBase[x, z].x = x;
            gridTilesBase[x, z].z = z;
            gridTilesBase[x, z].containId = containId;
            gridTilesBase[x, z].rotY = rotY;
        }
        else
        {
            gridTilesLevel[x, z].x = x;
            gridTilesLevel[x, z].z = z;
            gridTilesLevel[x, z].containId = containId;
            gridTilesLevel[x, z].rotY = rotY;
        }

 *     private GridTile[,] gridTilesBase;
    private GridTile[,] gridTilesLevel;

 * 
 
        gridTilesBase = new GridTile[size, size];
        gridTilesLevel = new GridTile[size, size];
        gridSize = size;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                gridTilesBase[i, j] = new();
                gridTilesLevel[i, j] = new();
            }
        }


    public string GetTileContainId(int index, GridTile[,] grid)
    {
        int iterate = 0;
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                if (index == iterate)
                {
                    return grid[i, j].contain;
                }
                iterate++;
            }
        }
        return null;
    }


    public void SetTileContainId(int index, int containId, GameEnums.TileLevel tile)
    {
        int iterate = 0;
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                if (index == iterate)
                {
                    if (tile == GameEnums.TileLevel.Base)
                    {
                        gridTilesBase[i, j].containId = containId;
                    }
                    else
                    {
                        gridTilesLevel[i, j].containId = containId;
                    }
                }
                iterate++;
            }
        }
    }




    public void PlaceTile(int index, string contain)
    {
        if (GetTileContain(index) == "empty")
        {
            SetTileContain(index, contain);
        }
        else
        {
            Debug.LogWarning("There is already an object there");
        }
    }

    public void DeleteTile(int index)
    {
        SetTileContain(index, "empty");
    }

        int iterate = 0;
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                if (index == iterate)
                {
                   
                }
                iterate++;
            }
        }
 
 */