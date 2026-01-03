using GameEnums;
using System;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    private PlaygroundGrid gridData;
    private Grid grid;
    private Transform centerPoint;

    private Dictionary<string, GameObject[,]> placedGameObjects = new();

    private int currentObjectId;
    private bool toggleBrush = false;
    private bool editingPlayground = false;
    private string currentSelectedTool = "";


    private string currentGridLayer;
    private List<string> gridLayers = new List<string>();

    public static event Action<string[]> OnToolChange;
    public static event Action<List<string>> OnLayerChange;
    public static event Action<ObjectsDatabase> OnItemsChange;
    public static event Action<bool, string> OnItemSelected;
    public static event Action<bool, string> OnMoveObjectStart;



    [SerializeField] private GameObject gridBasePrefab;
    [SerializeField] private GameObject gridVisualPrefab;
    [SerializeField] private Transform gridContainer;
    [SerializeField] private GameObject mouseIndicator;

    [SerializeField] private PreviewSystem previewSystem;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private ObjectManipulator objectManipulator;
    [SerializeField] private ObjectsDatabase objectsDatabase;

    [Space(10)]
    [Header("UI Objects: ")]
    [SerializeField] private TMP_Dropdown layerSelection;
    [SerializeField] private TMP_Dropdown itemSelection;
    [SerializeField] private TMP_Dropdown toolSelection;

    [Space(10)]
    [Header("Test Objects: ")]
    [SerializeField] private GameObject testObjectPrefab;
    [SerializeField] private TextMeshProUGUI layerText;
    [SerializeField] private TextMeshProUGUI selectedObjectText;
    [SerializeField] private TextMeshProUGUI selectedTool;


    public ObjectsDatabase ObjectsDatabase { get => objectsDatabase; set => objectsDatabase = value; }
    public int CurrentObjecyId { get => currentObjectId; set => currentObjectId = value; }
    public Transform GridContainer { get => gridContainer; set => gridContainer = value; }
    public PlaygroundGrid GridData { get => gridData; set => gridData = value; }
    public string CurrentGridLayer { get => currentGridLayer; set => currentGridLayer = value; }
    public Dictionary<string, GameObject[,]> PlacedGameObjects { get => placedGameObjects; set => placedGameObjects = value; }
    public List<string> GridLayers { get => gridLayers; set => gridLayers = value; }
    public Grid Grid { get => grid; set => grid = value; }

    ISelectionState selectionState;
    IBuildingState buildingState;

    private void OnEnable()
    {
        PlaygroundManager.OnStartPlaygroundEdit += CreateGrid;
    }

    private void OnDisable()
    {
        PlaygroundManager.OnStartPlaygroundEdit -= CreateGrid;
        ClearBuildTools();
    }

    private void CreateGrid(int size, int playgroundIndex)
    {
        //Create grid in data:
        //gridLayers.Add("Base");
        //gridData = new PlaygroundGrid(size, gridLayers[0]);

        SetupGrid(size);
        //Load the grid data on the grid on the world from previos sessions
        LoadGrid(playgroundIndex, size);
        GetCurrentLayer();
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
        selectionState = new SelectState(previewSystem, gridData, objectManipulator);
        previewSystem.StartShowingCursor(Vector2Int.one);
        inputManager.SetCameraTarget(centerPoint);
        inputManager.OnClicked += SelectObject;
    }


    private void LoadGrid(int playgroundIndex, int size)
    {
        Debug.Log("Index in GridManager: " + playgroundIndex + "Size in GridManager: " + size);
        int totalSize = GameManager.Instance.GameData.currentUser.playgrounds[playgroundIndex].plygrd_size;


        //Load the Index based Tile Array:
        List<TileDataArray> tileDataArray = GameManager.Instance.GameData.currentUser.playgrounds[playgroundIndex].tilesArray;

        //Initilize the Grid Objects:
        //placedGameObjects --> for in game objects
        //gridLayers --> for storing layers
        //gridData --> for storing grid data 
        placedGameObjects = new();
        gridLayers = new List<string>();
        gridData = new();
        gridData.GridSize = size;

        //Load Layers with empty arrays:
        foreach (var item in tileDataArray)
        {
            gridLayers.Add(item.layer);
            GridTile[,] newGridData = new GridTile[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    newGridData[i, j] = new();
                }
            }

            //Initilize all the layers with empty gameobject arrays:
            placedGameObjects.Add(item.layer, new GameObject[size, size]);

            //Fill all the data layers
            gridData.gridLayers.Add(item.layer, newGridData);
            for (int i = 0; i < totalSize; i++)
            {
                Debug.Log(i + " " + size + " " + item.tiles[i].tile_contain_id + " " + item.tiles[i].tile_rot_y + " " + item.layer);
                gridData.SetTileWithIndex(i, size, item.tiles[i].tile_contain_id, item.tiles[i].tile_rot_y, item.layer);
            }
            //Debug.Log("Get Contain Id: " + gridData.gridLayers[item.layer][0,0].containId);
            //Debug.Log("Grid Data on " + item.layer + ": Null?: " + gridData.gridLayers[item.layer] == null);
        }


        //Fill the layer gameobjects
        foreach (var item in gridLayers)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {

                    GridTile[,] gridTiles = gridData.gridLayers[item];
                    Debug.Log("The Layer: " + item + " On Cords: " + i + " / " + j + " The Id: " + gridTiles[i, j].containId + " The Rot: " + gridTiles[i, j]);
                    
                    int objectIdBase = gridTiles[i, j].containId;
                    currentObjectId = objectIdBase;
                    currentGridLayer = item;
                    objectManipulator.PlaceObject(new Vector3Int(gridTiles[i, j].x, 0, gridTiles[i, j].z));
                    //Debug.Log("The Object Rot: " + gridTiles[i, j].rotY);
                    objectManipulator.SetObjectRotation(new Vector3Int(gridTiles[i, j].x, 0, gridTiles[i, j].z), gridTiles[i, j].rotY, item);

                }
            }

        }

        currentGridLayer = gridLayers[0];
        OnLayerChange?.Invoke(gridLayers);
        OnItemsChange?.Invoke(objectsDatabase);
    }

    public void SaveGrid(int playgroundIndex)
    {
        int totalSize = gridData.GridSize * gridData.GridSize;
        int size = gridData.GridSize;
        List<TileDataArray> tileDataArray = new List<TileDataArray>();

        foreach (var item in gridLayers)
        {
            TileDataArray newTileDataArray = new TileDataArray();
            newTileDataArray.layer = item;
            newTileDataArray.tiles = new TileData[totalSize];
            //TileData[] tileData = new TileData[totalSize];
            for (int i = 0; i < totalSize; i++)
            {
                newTileDataArray.tiles[i] = new TileData();
                newTileDataArray.tiles[i].tile_index = i;
                newTileDataArray.tiles[i].tile_contain_id = gridData.GetTileWithIndex(i, size, item).containId;
                newTileDataArray.tiles[i].tile_rot_y = gridData.GetTileWithIndex(i, size, item).rotY;
            }

            tileDataArray.Add(newTileDataArray);

        }

        GameManager.Instance.GameData.currentUser.playgrounds[playgroundIndex].tilesArray = tileDataArray;
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
        selectionState = new SelectState(previewSystem, gridData, objectManipulator);
        inputManager.OnClicked += SelectObject;
    }

    public void GoBuildState()
    {
        ClearSelectTools();
        buildingState = new BuildingState(previewSystem, gridData, objectManipulator);
        SetTools();
        //SelectObject(0);
    }

    public void GoRemoveState()
    {
        ClearSelectTools();
        buildingState = new RemoveState(previewSystem, gridData, objectManipulator);
        SetTools();
        //SelectObject(0);
    }

    #region ItemControl

    private void GetCurrentObjectId()
    {
        //AssignObjectId(itemSelection.value);
        currentObjectId = itemSelection.value;
        selectedObjectText.text = "Object Id: " + currentObjectId.ToString();
    }

    public void AssignObjectId(int id)
    {
        currentObjectId = id;
        //int selectedObjectIndex = objectsDatabase.objectData.FindIndex(data => data.ID == id);
        selectedObjectText.text = "Object Id: " + currentObjectId.ToString();
        //Debug.Log("Object Chaged: " + currentObjectId + " Level: " + currentTileLevel);
    }


    #endregion

    #region LayerControl

    public void CreateNewLayer(string layer)
    {
        int size = gridData.GridSize;
        string newLayer = layer + (gridData.gridLayers.Count + 1);
        gridLayers.Add(newLayer);
        gridData.gridLayers.Add(gridLayers[gridLayers.Count - 1], new GridTile[size, size]); // Add the new layer to data
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                gridData.gridLayers[gridLayers[gridLayers.Count - 1]][i, j] = new();
            }
        }
        placedGameObjects.Add(gridLayers[gridLayers.Count - 1], new GameObject[size, size]); // Add the new layer in world
        OnLayerChange?.Invoke(gridLayers);
    }

    public void DeleteLayer()
    {
        string layer = currentGridLayer;
        gridData.gridLayers.Remove(layer);// remove layer from the data
        placedGameObjects.Remove(layer); // remove layer in world 
        gridLayers.Remove(layer);
        OnLayerChange?.Invoke(gridLayers);
        SetTools();
    }

    private void GetCurrentLayer()
    {
        currentGridLayer = gridLayers[layerSelection.value];
        layerText.text = currentGridLayer.ToString();
        //AssignObjectLayer(layerSelection.value);
    }

    #endregion

    #region ToolControl

    public void SetTools()
    {
        GetCurrentLayer();
        GetCurrentObjectId();
        GetCurrentTool();
        string[] tools = new string[3];
        tools[0] = currentGridLayer.ToString();
        tools[1] = currentObjectId.ToString();
        tools[2] = currentSelectedTool;

        OnToolChange?.Invoke(tools);
    }

    private void GetCurrentTool()
    {
        switch (toolSelection.value)
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
            for (int i = 0; i < gridData.GridSize; i++)
            {
                for (int j = 0; j < gridData.GridSize; j++)
                {
                    buildingState.OnDotAction(new Vector3Int(i, j, 0), currentObjectId, currentGridLayer);
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
            Debug.Log("Current Layer: " + currentGridLayer);
            if (placedGameObjects[currentGridLayer][GetGridPos().x, GetGridPos().z] != null)
            {
                GameObject currentGameobject = placedGameObjects[currentGridLayer][GetGridPos().x, GetGridPos().z].gameObject;
                selectionState.OnAction(GetGridPos(), currentGridLayer, currentGameobject);

                int id = gridData.gridLayers[currentGridLayer][GetGridPos().x, GetGridPos().z].containId;
                int selectedObjectIndex = objectsDatabase.objectData.FindIndex(data => data.ID == id);
                string name = objectsDatabase.objectData[selectedObjectIndex].Name;

                string info = "Selected Tile: " + GetGridPos() +
                    " \nContains: " + name +
                    " \nID: " + id;
                OnItemSelected?.Invoke(true, info);
            }
            else
            {
                Debug.Log("Nothing to select!");
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

 */