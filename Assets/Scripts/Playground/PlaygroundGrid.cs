using UnityEngine;

public class PlaygroundGrid
{
    private GridTile[,] gridTiles;
    private int gridSize;

    public int GridSize { get => gridSize; set => gridSize = value; }

    public PlaygroundGrid(int size)
    {
        gridTiles = new GridTile[size, size];
        gridSize = size;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                gridTiles[i, j] = new();
                gridTiles[i, j].x = i;
                gridTiles[i, j].z = j;
                gridTiles[i, j].contain = "empty";
                gridTiles[i, j].containId = -1;
            }
        }
    }

    public string GetTileContain(int index)
    {
        int iterate = 0;
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                if (index == iterate)
                {
                    return gridTiles[i, j].contain;
                }
                iterate++;
            }
        }
        return null;
    }

    public int[] GetTileCord(int index)
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


    public void SetTileContain(int index, string contain)
    {
        int iterate = 0;
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                if (index == iterate)
                {
                    gridTiles[i, j].contain = contain;
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


}


public class GridTile
{
    public int x;
    public int z;
    public string contain;
    public int containId;

}