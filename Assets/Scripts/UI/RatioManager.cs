using UnityEngine;
using GameEnums;

public class RatioManager : MonoBehaviour
{
    [SerializeField] private bool toggleLandscape = false;
    private PlaygroundUI activePlaygroundUI;

    private PlaygroundUI landscapePlaygroundUI;
    private PlaygroundUI portraitPlaygroundUI;

    private RotationLevel rotateLevel;

    public PlaygroundUI ActivePlaygroundUI { get => activePlaygroundUI; set => activePlaygroundUI = value; }

    private void Awake()
    {
        portraitPlaygroundUI = transform.GetChild(0).GetComponent<PlaygroundUI>();
        landscapePlaygroundUI = transform.GetChild(1).GetComponent<PlaygroundUI>();

        if (toggleLandscape)
        {
            AssignLandscapeMode();
        }
        else
        {
            AssignPortraitMode();
        }
    }

    private void Start()
    {
        if (GameManager.Instance.debug)
        {
            CreateDummyData();
        }

        portraitPlaygroundUI.PlaygroundUIStart();
        landscapePlaygroundUI.PlaygroundUIStart();
    }


    private void OnDisable()
    {
        transform.GetChild(0).GetComponent<PlaygroundUI>().UnsubscribeListeners();
        transform.GetChild(1).GetComponent<PlaygroundUI>().UnsubscribeListeners();
    }

    private void AssignLandscapeMode()
    {
        activePlaygroundUI = transform.GetChild(1).GetComponent<PlaygroundUI>();
        transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
        rotateLevel = RotationLevel.Landscape;
    }

    private void AssignPortraitMode()
    {
        activePlaygroundUI = transform.GetChild(0).GetComponent<PlaygroundUI>();
        transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
        rotateLevel = RotationLevel.Portrait;
    }

    public void RotateScreen()
    {
        if (rotateLevel == RotationLevel.Landscape)
        {
            //Turn Landscape to Portrait
            AssignPortraitMode();
        }
        else
        {
            //Turn Portrait to Landscape
            AssignLandscapeMode();
        }
    }


    public void SelectPlayground()
    {
        activePlaygroundUI.SelectPlayground();
    }

    private void CreateDummyData()
    {
        GameData gameData = new GameData();
        gameData.currentUser = new();
        gameData.currentUser.username = "Test User";
        gameData.currentUser.curr_ply_index = 0;
        gameData.currentUser.playgrounds = new();
        PlaygroundData testPlayground = new();
        testPlayground.plygrd_name = "TestPlayground";
        testPlayground.plygrd_desc = "This playground is for testing";
        testPlayground.plygrd_size = 25;
        testPlayground.tiles_arrays = new();
        TileDataArray tileDataArray = new TileDataArray();
        tileDataArray.tiles = new TileData[testPlayground.plygrd_size];
        tileDataArray.tile_layer = "Base";

        for (int i = 0; i < testPlayground.plygrd_size; i++)
        {
            TileData newTile = new TileData();
            newTile.tile_index = i;
            newTile.tile_contain_id = -1;
            newTile.tile_rot_y = 0;
            tileDataArray.tiles[i] = newTile;
        }
        testPlayground.tiles_arrays.Add(tileDataArray);
        GameManager.Instance.GameData = gameData;
    }

}
