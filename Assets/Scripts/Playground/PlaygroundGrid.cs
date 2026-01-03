using System.Collections.Generic;
using UnityEngine;

public class PlaygroundGrid
{
    public Dictionary<string, GridTile[,]> gridLayers = new Dictionary<string, GridTile[,]>();
    private int gridSize;

    public int GridSize { get => gridSize; set => gridSize = value; }

    public PlaygroundGrid(int size, string layer)
    {

        GridTile[,] newGrid = new GridTile[size, size];
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                newGrid[i, j] = new();
            }
        }

        gridLayers.Add(layer, newGrid);
    }

    public PlaygroundGrid() { }


    public GridTile GetTileWithIndex(int index, int size, string layer)
    {
        int x = index % size;
        int z = index / size;
        GridTile[,] currentTileArr = gridLayers[layer];
        return currentTileArr[x, z];
    }

    public void SetTileWithIndex(int index, int size, int containId, int rotY, string layer)
    {
        int x = index % size;
        int z = index / size;


        Debug.Log(index + " " + size + " " + containId + " " + rotY + " " + layer);
        //Debug.Log("The Cords: x: " + x + " / z: " + z);
        //Debug.Log("Is Grid Layer Null: " + gridLayers[layer][x,z] == null);

        gridLayers[layer][x, z].x = x;
        gridLayers[layer][x, z].z = z;
        gridLayers[layer][x, z].index = index;
        gridLayers[layer][x, z].containId = containId;
        gridLayers[layer][x, z].rotY = rotY;

    }

    public void SetTileWithCord(int x, int z, int containId,int rotY, string layer)
    {
        gridLayers[layer][x, z].containId = containId;
        gridLayers[layer][x, z].rotY = rotY;
    }

    public int GetTileIndex(int x, int z, string layer)
    {
        return gridLayers[layer][x, z].index;
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