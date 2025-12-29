using UnityEngine;

public class PlaygroundGrid
{
    private GridTile[,] gridTilesBase;
    private GridTile[,] gridTilesLevel;

    private int gridSize;

    public int GridSize { get => gridSize; set => gridSize = value; }
    public GridTile[,] GridTilesBase { get => gridTilesBase; set => gridTilesBase = value; }
    public GridTile[,] GridTilesLevel { get => gridTilesLevel; set => gridTilesLevel = value; }

    public PlaygroundGrid(int size)
    {
        gridTilesBase = new GridTile[size, size];
        gridTilesLevel = new GridTile[size, size];
        gridSize = size;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                gridTilesBase[i, j] = new();
                gridTilesBase[i, j].x = i;
                gridTilesBase[i, j].z = j;
                gridTilesBase[i, j].contain = "empty";
                gridTilesBase[i, j].containId = -1;

                gridTilesLevel[i, j] = new();
                gridTilesLevel[i, j].x = i;
                gridTilesLevel[i, j].z = j;
                gridTilesLevel[i, j].contain = "empty";
                gridTilesLevel[i, j].containId = -1;
            }
        }
    }

    public string GetTileContain(int index, GridTile[,] grid)
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


    public void SetTileContain(int index, string contain, GridTile[,] grid)
    {
        int iterate = 0;
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                if (index == iterate)
                {
                    grid[i, j].contain = contain;
                }
                iterate++;
            }
        }
    }

    

}


public class GridTile
{
    public int x;
    public int z;
    public string contain;
    public int containId;

}

/*
 
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

 
 */