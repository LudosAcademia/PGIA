using System;
using System.Drawing;
using UnityEngine;

public class PlaygroundManager : MonoBehaviour
{

    public static event Action OnStartPlaygroundCreate;
    public static event Action OnCancelPlaygroundCreate;
    public static event Action OnEndPlaygroundCreate;


    //Starts the creation process for a new playground
    public void StartCreatePlayground()
    {
        OnStartPlaygroundCreate?.Invoke();
    }

    //Cancels the creation process for a new playground
    public void CancelCreatePlayground()
    {
        OnCancelPlaygroundCreate?.Invoke();
    }

    public void EndCreatePlayground()
    {
        OnEndPlaygroundCreate?.Invoke();
    }



}

/*
 
       private Vector3Int lastDetectedPosition = Vector3Int.zero;
    [SerializeField] private Grid grid;
    private PlaygroundGrid playgroundGrid;
    public void CreateGrid()
    {
        //Creates a grid that is 10x10
        playgroundGrid = new PlaygroundGrid(10);
        Debug.Log("Grid is created");
    }

    public void TestGetTile(int index)
    {
        for (int i = 0; i < index; i++)
        {
            Debug.Log($"The tile {i} contains " + playgroundGrid.GetTileContain(i));
        }
    }

    public void TestSetTile()
    {
        int size = playgroundGrid.GridSize * playgroundGrid.GridSize;
        for (int i = 0; i < size; i++)
        {
            playgroundGrid.PlaceTile(i, "chair");
        }
        Debug.Log("Grid created and chairs placed");
        for (int i = 0; i < size; i++)
        {
            if (playgroundGrid.GetTileContain(i) != "empty")
            {
                GameObject newChair = Instantiate(chairObject);
                newChair.transform.position = new Vector3(playgroundGrid.GetTileCord(i)[0], 0, playgroundGrid.GetTileCord(i)[1]);
            }
        }
    }

 
 */