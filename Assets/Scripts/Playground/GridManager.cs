using GameEnums;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    private PlaygroundGrid playgroundGrid;
    private Grid grid;
    private Transform centerPoint;

    private int currentObjectId;
    private bool toggleBrush = false;
    private bool editingPlayground = false;
    private string currentSelectedTool = "";
    private int currentSelectedToolId = 0;


    private string currentGridLayer;
    public static event Action OnBuildingSection;
    public static event Action OnSelectionSection;

    public static event Action<string[]> OnToolChange;
    public static event Action<List<string>> OnLayerChange;
    public static event Action<ObjectsDatabase> OnItemsChange;
    public static event Action<bool, string> OnItemSelected;
    public static event Action<bool, string> OnMoveObjectStart;
    public static event Action OnGridConstFinished;


    [SerializeField] private GameObject gridBasePrefab;
    [SerializeField] private GameObject gridVisualPrefab;
    [SerializeField] private Transform gridContainer;
    [SerializeField] private GameObject mouseIndicator;

    [SerializeField] private PreviewSystem previewSystem;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private ObjectManipulator objectManipulator;
    [SerializeField] private ObjectsDatabase objectsDatabase;

    [Space(10)]
    [Header("Test Objects: ")]
    [SerializeField] private GameObject testObjectPrefab;
    [SerializeField] private TextMeshProUGUI layerText;
    [SerializeField] private TextMeshProUGUI selectedObjectText;
    [SerializeField] private TextMeshProUGUI selectedTool;


    public ObjectsDatabase ObjectsDatabase { get => objectsDatabase; set => objectsDatabase = value; }
    public int CurrentObjecyId { get => currentObjectId; set => currentObjectId = value; }
    public Transform GridContainer { get => gridContainer; set => gridContainer = value; }
    public string CurrentGridLayer { get => currentGridLayer; set => currentGridLayer = value; }
    public Grid Grid { get => grid; set => grid = value; }
    public PlaygroundGrid PlaygroundGrid { get => playgroundGrid; set => playgroundGrid = value; }

    ISelectionState selectionState;
    IBuildingState buildingState;

    private void OnEnable()
    {
        PlaygroundManager.OnStartPlaygroundEdit += CreateGrid;
        PlaygroundUI.OnItemChange += ChangeCurrenItem;
        PlaygroundUI.OnToolChange += ChangeCurrenTool;
        PlaygroundUI.OnLayerChange += ChangeCurrentLayer;
    }

    private void OnDisable()
    {
        PlaygroundManager.OnStartPlaygroundEdit -= CreateGrid;
        PlaygroundUI.OnItemChange -= ChangeCurrenItem;
        PlaygroundUI.OnToolChange -= ChangeCurrenTool;
        PlaygroundUI.OnLayerChange -= ChangeCurrentLayer;
        ClearBuildTools();
    }

    private void CreateGrid(int size, int playgroundIndex)
    {

        SetupGrid(size);

        //Load the grid data on the grid on the world from previos sessions
        LoadGrid(playgroundIndex, size);
    }

    private void SetupGrid(int size)
    {
        //Create the Grid in game world:
        Vector3 gridPos = new Vector3(size / 2, 0, size / 2);

        //Setup gridbase:
        GameObject gridBase = Instantiate(gridBasePrefab, transform.position, Quaternion.identity);
        gridBase.transform.SetParent(gridContainer);
        gridBase.transform.GetChild(0).transform.localScale = new Vector3(size, size, 1);
        gridBase.transform.GetChild(0).transform.position = gridPos;

        //Setup grid visuals that holds the tiles visuals
        GameObject gridVisual = Instantiate(gridVisualPrefab, transform.position, Quaternion.identity);
        gridVisual.transform.SetParent(gridContainer);
        gridVisual.transform.GetChild(0).transform.localScale = new Vector3(size, size, size);
        gridVisual.transform.GetChild(0).transform.position = gridPos;
        grid = gridBase.GetComponent<Grid>();
        centerPoint = gridBase.transform.GetChild(0).GetChild(0).transform;

        //Set up Selection state:
        previewSystem.StartShowingCursor(Vector2Int.one);
        inputManager.SetCameraTarget(centerPoint);
    }

    private void LoadGrid(int playgroundIndex, int size)
    {

        //Load the Index based Tile Array:
        List<TileDataArray> tileDataArray = GameManager.Instance.GameData.currentUser.playgrounds[playgroundIndex].tiles_arrays;
        int totalSize = size * size;
        List<string> gridLayerKeys = new List<string>();

        //Load the layer keys:
        foreach (var item in tileDataArray)
        {
            gridLayerKeys.Add(item.tile_layer);
        }

        //Initilizer playgroundGrid:
        playgroundGrid = new(size, gridLayerKeys);
        playgroundGrid.GridSize = size;
        //will change
        playgroundGrid.logicReadyItems = new();

        for (int layerIndex = 0; layerIndex < tileDataArray.Count; layerIndex++)
        {
            for (int i = 0; i < totalSize; i++)
            {
                int index = tileDataArray[layerIndex].tiles[i].tile_index;
                int x = index % size;
                int z = index / size;
                int rotY = tileDataArray[layerIndex].tiles[i].tile_rot_y;
                int containId = tileDataArray[layerIndex].tiles[i].tile_contain_id;
                GridTile loadTile = new GridTile(x, z, index, rotY, containId);
                playgroundGrid.SetDataIndex(loadTile, index, size, tileDataArray[layerIndex].tile_layer);
            }
        }

        //Fill the layer gameobjects and data:
        foreach (var layer in gridLayerKeys)
        {
            GridTile[,] gridTiles = playgroundGrid.grid[layer].data;

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    int objectId = gridTiles[i, j].containId;
                    currentObjectId = objectId;
                    currentGridLayer = layer;
                    objectManipulator.PlaceObjectWithRotation(new Vector3Int(gridTiles[i, j].x, 0, gridTiles[i, j].z), gridTiles[i, j].rotY);
                }
            }
        }


        currentGridLayer = gridLayerKeys[0];
        OnLayerChange?.Invoke(gridLayerKeys);
        OnItemsChange?.Invoke(objectsDatabase);
        OnGridConstFinished?.Invoke();
    }

    public void SaveGrid(int playgroundIndex)
    {
        int totalSize = playgroundGrid.GridSize * playgroundGrid.GridSize;
        int size = playgroundGrid.GridSize;
        List<TileDataArray> tileDataArray = new List<TileDataArray>();
        Debug.Log(PrintSaveFile(playgroundIndex));
        foreach (var item in playgroundGrid.gridLayerKeys)
        {
            TileDataArray newTileDataArray = new TileDataArray();
            newTileDataArray.tile_layer = item;
            newTileDataArray.tiles = new TileData[totalSize];
            //TileData[] tileData = new TileData[totalSize];
            for (int i = 0; i < totalSize; i++)
            {
                newTileDataArray.tiles[i] = new TileData();
                newTileDataArray.tiles[i].tile_index = i;
                newTileDataArray.tiles[i].tile_contain_id = playgroundGrid.GetDataIndex(i, size, item).containId;
                newTileDataArray.tiles[i].tile_rot_y = playgroundGrid.GetDataIndex(i, size, item).rotY;
            }

            tileDataArray.Add(newTileDataArray);

        }
        GameManager.Instance.GameData.currentUser.playgrounds[playgroundIndex].tiles_arrays = tileDataArray;
    }

    private string PrintSaveFile(int playgroundIndex)
    {
        int totalSize = playgroundGrid.GridSize * playgroundGrid.GridSize;
        int size = playgroundGrid.GridSize;
        List<TileDataArray> tileDataArray = new List<TileDataArray>();
        string saveFile = "no save file";

        foreach (var item in playgroundGrid.gridLayerKeys)
        {
            TileDataArray newTileDataArray = new TileDataArray();
            newTileDataArray.tile_layer = item;
            newTileDataArray.tiles = new TileData[totalSize];
            //TileData[] tileData = new TileData[totalSize];
            for (int i = 0; i < totalSize; i++)
            {
                saveFile += "\n Save Tile: \n At:" + i + "\n Contain: " +
                playgroundGrid.GetDataIndex(i, size, item).containId + "\n Rotation: " +
                playgroundGrid.GetDataIndex(i, size, item).rotY + "\n At Cords: " + "\n X: " +
                playgroundGrid.GetDataIndex(i, size, item).x + " Z: " +
                playgroundGrid.GetDataIndex(i, size, item).z;

            }
        }

        return saveFile;

    }


    private void Update()
    {
        if (editingPlayground)
        {
            //Debug.Log("YOU CANT GO IN HERE");
            EdittingPlayground();
        }
    }

    public void TogglePlaygroundUpdate(bool set)
    {
        editingPlayground = set;
    }

    private void EdittingPlayground()
    {
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        //Debug.Log("Pos: " + mousePosition);
        mouseIndicator.transform.position = mousePosition;

        if (toggleBrush)
        {
            BuildObject();
        }

        if (selectionState != null)
        {
            selectionState.UpdateState(gridPosition);// previewSystem.UpdateCursor(gridPosition);
        }
        else if (buildingState != null)
        {
            buildingState.UpdateState(gridPosition);
        }
        else
        {
            return;
        }
    }

    public void BuildingSection()
    {
        TogglePlaygroundUpdate(true);
        GoBuildState();
        OnBuildingSection?.Invoke();
    }

    public void EmptyStateSection()
    {
        TogglePlaygroundUpdate(false);
        ClearSelectTools();
        ClearBuildTools();
        selectionState = null;
        buildingState = null;
    }

    public void GoSelectState()
    {
        ClearBuildTools();
        buildingState = null;
        selectionState = new SelectState(previewSystem, playgroundGrid, objectManipulator);
        inputManager.OnClicked += SelectObject;
        OnSelectionSection?.Invoke();
    }

    public void GoBuildState()
    {
        ClearSelectTools();
        buildingState = new BuildingState(previewSystem, playgroundGrid, objectManipulator);
        SetToolNames();
        SetCurrentTool();
        //SelectObject(0);
    }

    public void GoRemoveState()
    {
        ClearSelectTools();
        buildingState = new RemoveState(previewSystem, playgroundGrid, objectManipulator);
        SetToolNames();
        //SelectObject(0);
    }

    private void ChangeCurrenItem(int itemId)
    {
        //AssignObjectId(itemSelection.value);
        currentObjectId = itemId;
        selectedObjectText.text = "Object Id: " + currentObjectId.ToString();
    }

    private void ChangeCurrenTool(int toolId)
    {
        currentSelectedToolId = toolId;
        if (buildingState != null)
        {
            SetCurrentTool();
        }
    }

    private void ChangeCurrentLayer(int layerIndex)
    {
        currentGridLayer = playgroundGrid.gridLayerKeys[layerIndex];
        //Debug.Log(currentGridLayer);
        layerText.text = currentGridLayer.ToString();
    }


    #region LayerControl

    public void CreateNewLayer(string layer)
    {
        int size = playgroundGrid.GridSize;
        string newLayer = layer + (playgroundGrid.grid.Count + 1);
        playgroundGrid.AddLayer(newLayer);
        OnLayerChange?.Invoke(playgroundGrid.gridLayerKeys);
    }

    public void DeleteLayer()
    {
        string layer = currentGridLayer;
        if (playgroundGrid.gridLayerKeys.Count == 1) { return; }

        playgroundGrid.DeleteLayer(layer);
        OnLayerChange?.Invoke(playgroundGrid.gridLayerKeys);
        SetToolNames();
        currentGridLayer = playgroundGrid.gridLayerKeys[0];
    }


    #endregion

    #region ToolControl

    public void SetToolNames()
    {
        string[] tools = new string[3];
        tools[0] = currentGridLayer.ToString();
        tools[1] = currentObjectId.ToString();
        tools[2] = currentSelectedTool;

        OnToolChange?.Invoke(tools);
    }

    private void SetCurrentTool()
    {
        switch (currentSelectedToolId)
        {
            case 0:
                SelectDot();
                break;
            case 1:
                SelectBrush();
                break;
            case 2:
                SelectFill();
                break;
        }
    }

    private void BrushToggleStart()
    {
        toggleBrush = true;
    }

    private void BrushToggleEnd()
    {
        toggleBrush = false;
    }

    public void SelectBrush()
    {
        ClearBuildTools();
        inputManager.OnClickStarted += BrushToggleStart;
        inputManager.OnClickEnded += BrushToggleEnd;
        currentSelectedTool = "Brush";

    }

    public void SelectDot()
    {
        ClearBuildTools();
        inputManager.OnClicked += BuildObject;
        currentSelectedTool = "Dot";
    }

    public void SelectFill()
    {
        ClearBuildTools();
        inputManager.OnClicked += BuildAllObjects;
        currentSelectedTool = "Fill";
    }

    #endregion

    #region Building

    private void BuildObject()
    {
        if (inputManager.IsMouseOnGrid() && !inputManager.IsPointerOverUI())
        {
            buildingState.OnDotAction(GetGridPos(), currentObjectId, currentGridLayer);
        }
    }


    private void BuildAllObjects()
    {
        if (inputManager.IsMouseOnGrid() && !inputManager.IsPointerOverUI())
        {
            //Debug.Log("The Grid Size in Build All Object: " + gridData.GridSize);
            for (int i = 0; i < playgroundGrid.GridSize; i++)
            {
                for (int j = 0; j < playgroundGrid.GridSize; j++)
                {
                    buildingState.OnDotAction(new Vector3Int(i, 0, j), currentObjectId, currentGridLayer);
                }
            }
        }
    }

    #endregion

    #region Selection
    private void SelectObject()
    {
        if (inputManager.IsMouseOnGrid() && !inputManager.IsPointerOverUI())
        {
            //Debug.Log("Current Layer: " + currentGridLayer);
            if (playgroundGrid.grid[currentGridLayer].visuals[GetGridPos().x, GetGridPos().z] != null)
            {
                GameObject currentGameobject = playgroundGrid.grid[currentGridLayer].visuals[GetGridPos().x, GetGridPos().z].gameObject;
                selectionState.OnAction(GetGridPos(), currentGridLayer, currentGameobject);

                int id = playgroundGrid.grid[currentGridLayer].data[GetGridPos().x, GetGridPos().z].containId;
                int selectedObjectIndex = objectsDatabase.objectData.FindIndex(data => data.ID == id);
                string name = objectsDatabase.objectData[selectedObjectIndex].Name;

                string info = "Selected Tile: " + GetGridPos() +
                    " \nContains: " + name +
                    " \nID: " + id;
                OnItemSelected?.Invoke(true, info);
            }
            else
            {
                //Debug.Log("Nothing to select!");
                OnItemSelected?.Invoke(false, "");
                selectionState.EndState();
            }
        }
    }

    public void MoveObjectStart()
    {
        selectionState.OnMoveStart();
        inputManager.OnClicked += MoveObjectEnd;
        OnMoveObjectStart?.Invoke(true, "Click On Grid To Re-locate the selected Object");
    }

    private void MoveObjectEnd()
    {
        if (inputManager.IsMouseOnGrid() && !inputManager.IsPointerOverUI())
        {
            selectionState.OnMoveEnd(GetGridPos(), currentGridLayer);
            inputManager.OnClicked -= MoveObjectEnd;
            OnMoveObjectStart?.Invoke(false, "");
        }
    }

    public void RotateObject()
    {
        selectionState.OnRotate(GetGridPos());
    }



    #endregion


    public void ClearBuildTools()
    {
        inputManager.OnClickStarted -= BrushToggleStart;
        inputManager.OnClickEnded -= BrushToggleEnd;
        inputManager.OnClicked -= BuildAllObjects;
        inputManager.OnClicked -= BuildObject;
    }
    public void ClearSelectTools()
    {
        inputManager.OnClicked -= SelectObject;
    }

    private Vector3Int GetGridPos()
    {
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        return gridPosition;
    }


}


/*
 * 
 *     private void PrintArray(GridTile[,] gridData, GameObject[,] placedData)
    {
        for (int i = 0; i < gridData.Length; i++)
        {
            for (int j = 0; j < gridData.Length; j++)
            {
                Debug.Log("The Tile " + gridData[i, j].index + ": " + gridData[i, j].containId);
                Debug.Log("The Tile " + gridData[i, j].index + ": " + gridData[i, j].containId);

            }
        }
    }

 * 
        for (int i = 0; i < totalSize; i++)
        {
            foreach (var item in gridLayers)
            {
                tileDataArray[]
                tileDataBase[i] = new TileData();
                tileDataBase[i].tile_index = i;
                tileDataBase[i].tile_contain_id = gridData.GetTile(i, size, TileLevel.Base).containId;
                tileDataBase[i].tile_rot_y = gridData.GetTile(i, size, TileLevel.Base).rotY;


                int objectIdBase = gridTiles[i, j].containId;
                currentObjectId = objectIdBase;
                currentTileLevel = TileLevel.Base;
                objectPlacer.PlaceObject(new Vector3Int(gridTiles[i, j].x, 0, gridTiles[i, j].z));
            }



            tileDataLevel[i] = new TileData();
            tileDataLevel[i].tile_index = i;
            tileDataLevel[i].tile_contain_id = gridData.GetTile(i, size, TileLevel.Level).containId;
            tileDataLevel[i].tile_rot_y = gridData.GetTile(i, size, TileLevel.Level).rotY;

        }

 * 
 * 
 * 
 * 
 * 
 * 
         for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                if (playgroundGrid.GridTiles[i, j].contain == "empty")
                {
                    Vector3Int pos = new Vector3Int(playgroundGrid.GridTiles[i, j].x, 0, playgroundGrid.GridTiles[i, j].z);

                }
            }
        }
 

        Vector3Int centerCell = new Vector3Int(gridSize / 2, gridSize / 2, 0);
        Vector3 centerWorld = grid.GetCellCenterWorld(centerCell);

 Debug.Log("Inside Save Grid: " + 
                    " ContainID: " + gridData.GetTileWithIndex(i, size, item).containId +
                    " RotY: " + gridData.GetTileWithIndex(i, size, item).rotY +
                    " On Cords: " + gridData.GetTileWithIndex(i, size, item).x + " / " + gridData.GetTileWithIndex(i, size, item).z);

 */