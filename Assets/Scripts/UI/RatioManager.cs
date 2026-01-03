using UnityEngine;

public class RatioManager : MonoBehaviour
{
    [SerializeField] private bool toggleLandscape = false;
    private PlaygroundUI activePlaygroundUI;

    public PlaygroundUI ActivePlaygroundUI { get => activePlaygroundUI; set => activePlaygroundUI = value; }

    private void Awake()
    {
        if (toggleLandscape)
        {
            activePlaygroundUI = transform.GetChild(1).GetComponent<PlaygroundUI>();
            transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            activePlaygroundUI = transform.GetChild(0).GetComponent<PlaygroundUI>();
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    private void Start()
    {
        if (GameManager.Instance.debug)
        {
            CreateDummyData();
        }

        activePlaygroundUI.PlaygroundUIStart();
    }

    public void SelectPlayground()
    {
        activePlaygroundUI.SelectPlayground();
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
