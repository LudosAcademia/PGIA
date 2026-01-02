using System;
using UnityEngine;

public class PlaygroundManager : MonoBehaviour
{
    [SerializeField] private PlaygroundUI playgroundUI;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private GameObject gridContainer;
    [SerializeField] private GameObject vCam;

    public GridManager GridManager { get => gridManager; set => gridManager = value; }

    public static event Action<int, int> OnStartPlaygroundEdit;
    public static event Action OnCancelPlaygroundEdit;
    public static event Action<PlaygroundData> OnEndPlaygroundEdit;

    public static event Action OnStartPlaygroundDelete;
    public static event Action<PlaygroundData> OnEndPlaygroundDelete;


    public static event Action OnStartPlaygroundCreate;
    public static event Action OnCancelPlaygroundCreate;
    public static event Action OnEndPlaygroundCreate;

    private void Start()
    {
        if (GameManager.Instance.debug)
        {
            CreateDummyData();
        }
        playgroundUI.PlaygroundUIStart();

    }



    public void StartEditPlayground()
    {
        int index = GameManager.Instance.GameData.currentUser.curr_ply_index;
        int totalSize = GameManager.Instance.GameData.currentUser.playgrounds[index].plygrd_size;
        int size = (int)Math.Sqrt(totalSize);
        Debug.Log("The index: " + index + "The Size: " + size);
        if (gridContainer.transform.childCount != 0)
        {
            for (int i = 0; i < gridContainer.transform.childCount; i++)
            {
                Destroy(gridContainer.transform.GetChild(i).gameObject);
            }
        }
        OnStartPlaygroundEdit?.Invoke(size, index);
        vCam.SetActive(true);
    }


    /*

     
     */

    public void CancelEditPlayground()
    {
        vCam.SetActive(false);
        if (gridContainer.transform.childCount != 0)
        {
            for (int i = 0; i < gridContainer.transform.childCount; i++)
            {
                Destroy(gridContainer.transform.GetChild(i).gameObject);
            }
        }
        OnCancelPlaygroundEdit?.Invoke();
    }

    public void EndEditPlayground()
    {
        vCam.SetActive(false);
        int index = GameManager.Instance.GameData.currentUser.curr_ply_index;
        gridManager.SaveGrid(index);
        GameManager.Instance.GameData.currentUser.playgrounds[index].update = true;
        OnEndPlaygroundEdit?.Invoke(GameManager.Instance.GameData.currentUser.playgrounds[index]);
    }


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


    private void CreateDummyData()
    {
        GameData gameData = new GameData();
        gameData.currentUser = new();
        gameData.currentUser.name = "Test User";
        gameData.currentUser.curr_ply_index = 0;
        gameData.currentUser.playgrounds = new();
        PlaygroundData testPlayground = new();
        testPlayground.plygrd_name = "TestPlayground";
        testPlayground.plygrd_desc = "This playground is for testing";
        testPlayground.plygrd_size = 25;
        testPlayground.tilesArray = new();
        TileDataArray tileDataArray = new TileDataArray();
        tileDataArray.tiles = new TileData[testPlayground.plygrd_size];
        tileDataArray.layer = "Base";

        for (int i = 0; i < testPlayground.plygrd_size; i++)
        {
            TileData newTile = new TileData();
            newTile.tile_index = i;
            newTile.tile_contain_id = -1;
            newTile.tile_rot_y = 0;
            tileDataArray.tiles[i] = newTile;
        }
        testPlayground.tilesArray.Add(tileDataArray);
        GameManager.Instance.GameData = gameData;
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