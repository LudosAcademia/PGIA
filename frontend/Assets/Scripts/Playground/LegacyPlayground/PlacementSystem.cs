
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{

    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;
    [SerializeField] private ObjectsDatabase database;
    [SerializeField] private GameObject gridVisualization;
    [SerializeField] private PreviewSystem previewSystem;
    [SerializeField] private Transform objectParent;

    IBuildingState buildingState;


    private GridData basePropData, levelPropData;
    private Vector3Int lastDetectedPosition = Vector3Int.zero;
    [SerializeField] private ObjectPlacer objectPlacer;

    private void Awake()
    {
        GameManager.Instance.GetGameData();
    }

    private void Start()
    {
        StopPlacement();
        basePropData = new();
        levelPropData = new();
    }

    private void Update()
    {
        if (buildingState == null) { return; }

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        if (lastDetectedPosition != gridPosition)
        {
            buildingState.UpdateState(gridPosition);
            lastDetectedPosition = gridPosition;
        }

    }

    public void StartPlayground()
    {
        PlaygroundData newPlayground = new PlaygroundData();
        UserData userData = GameManager.Instance.GetGameData().currentUser;

    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        buildingState = new PlacementState(ID, grid, previewSystem, database, basePropData, levelPropData, objectPlacer);

        gridVisualization.SetActive(true);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
        inputManager.OnRotate += RotateStructure;

    }

    public void StartRemoving()
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        buildingState = new RemovingState(grid, previewSystem, basePropData, levelPropData, objectPlacer);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    private void PlaceStructure()
    {
        if (inputManager.IsPointerOverUI()) { return; }

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        buildingState.OnAction(gridPosition);

    }

    private void RotateStructure(int dir)
    {

    }


    private void StopPlacement()
    {
        if (buildingState == null) { return; }
        gridVisualization.SetActive(false);
        buildingState.EndState();
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        inputManager.OnRotate -= RotateStructure;
        lastDetectedPosition = Vector3Int.zero;
        buildingState = null;
    }


}

/*
         //bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        //previewRenderer.material.color = placementValidity ? Color.white : Color.red;
        //Debug.Log("Placement: " + placementValidity);

        //mouseIndicator.transform.position = mousePosition;
        //cellIndicator.transform.position = grid.CellToWorld(gridPosition);
        //previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
        //lastDetectedPosition = gridPosition;
         //cellIndicator.SetActive(false);

        Debug.Log("GameManager: " + (GameManager.Instance != null));
        Debug.Log("GameData: " + (GameManager.Instance?.GetGameData() != null));
        Debug.Log("CurrentUser: " + (GameManager.Instance?.GetGameData()?.currentUser != null));
        Debug.Log("PlaygroundDatas: " + (GameManager.Instance?.GetGameData()?.currentUser?.playgroundDatas != null));



        for (int i = 0; i < objectParent.childCount; i++)
        {
            TileData newProp = new TileData();
            newProp.iD = int.Parse(objectParent.GetChild(i).name);
            newProp.position = objectParent.GetChild(i).position;
            newProp.rotation = Vector3.one;
            //newPlayground.placedProps.Add(newProp);
        }


        userData.playgrounds.Add(newPlayground);
        newPlayground.id = userData.playgrounds.Count;
        userData.curr_ply_index = newPlayground.id - 1;
        //Debug.Log(" newPlayground.iD: " + newPlayground.iD + " userData.currentPlaygroundIndex : " + userData.currentPlaygroundIndex);

        GameManager.Instance.GetGameData().userData[GameManager.Instance.GetGameData().currentUserIndex] = userData;
        GameManager.Instance.SaveGame();
        GameManager.Instance.GoToLevel("PlaygroundScene");


 */