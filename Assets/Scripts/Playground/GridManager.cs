using UnityEngine;

public class GridManager : MonoBehaviour
{
    private PlaygroundGrid gridData;
    private Grid grid;
    private Transform centerPoint;
    private int currentObjectId;
    private GameEnums.TileLevel currentTileLevel;


    [SerializeField] private int gridSize = 5;
    [SerializeField] private GameObject gridBasePrefab;
    [SerializeField] private GameObject gridVisualPrefab;
    [SerializeField] private Transform gridContainer;
    [SerializeField] private GameObject mouseIndicator;

    [SerializeField] private PreviewSystem previewSystem;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private ObjectPlacer objectPlacer;

    [Space(10)]


    [Header("Test Objects: ")]
    [SerializeField] private GameObject testObjectPrefab;

    ISelectionState selectionState;
    IBuildingState buildingState;

    private void OnEnable()
    {
        inputManager.OnClicked += SelectObject;
        inputManager.OnClicked += PlaceOject;

    }

    private void OnDisable()
    {
        inputManager.OnClicked -= SelectObject;
        inputManager.OnClicked -= PlaceOject;

    }

    private void CreateGrid(int size)
    {
        gridData = new PlaygroundGrid(size);
        Vector3 gridPos = new Vector3(size / 2, 0, size / 2);
        GameObject gridBase = Instantiate(gridBasePrefab, transform.position, Quaternion.identity);
        gridBase.transform.SetParent(gridContainer);
        gridBase.transform.GetChild(0).transform.localScale = new Vector3(size, size, 1);
        gridBase.transform.GetChild(0).transform.position = gridPos;
        GameObject gridVisual = Instantiate(gridVisualPrefab, transform.position, Quaternion.identity);
        gridVisual.transform.SetParent(gridContainer);
        gridVisual.transform.GetChild(0).transform.localScale = new Vector3(size, size, size);
        gridVisual.transform.GetChild(0).transform.position = gridPos;
        grid = gridBase.GetComponent<Grid>();
        centerPoint = gridBase.transform.GetChild(0).GetChild(0).transform;

        selectionState = new SelectState(grid, previewSystem, gridData);
        previewSystem.StartShowingCursor(Vector2Int.one);
        inputManager.SetCameraTarget(centerPoint);
    }

    private void Start()
    {
        CreateGrid(gridSize);
    }

    private void Update()
    {
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        //Debug.Log("Pos: " + mousePosition);
        mouseIndicator.transform.position = mousePosition;

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

    public void GoSelectState()
    {
        selectionState = new SelectState(grid, previewSystem, gridData);
        buildingState = null;
        inputManager.OnClicked += SelectObject;
        inputManager.OnClicked -= PlaceOject;
    }

    public void GoBuildState()
    {
        buildingState = new BuildingState(grid, previewSystem, gridData, objectPlacer);
        selectionState = null;
        inputManager.OnClicked -= SelectObject;
        inputManager.OnClicked += PlaceOject;
    }

    public void SelectObject(int id)
    {
        currentObjectId = id;

        if (id == 0)
        {
            currentTileLevel = GameEnums.TileLevel.Level;

        }
        else
        {
            currentTileLevel = GameEnums.TileLevel.Base;
        }
    }


    private void PlaceOject()
    {
        if (inputManager.IsMouseOnGrid())
        {
            Vector3 mousePosition = inputManager.GetSelectedMapPosition();
            Vector3Int gridPosition = grid.WorldToCell(mousePosition);
            buildingState.OnDotAction(gridPosition, currentObjectId, currentTileLevel);
        }
    }

    private void SelectObject()
    {
        if (inputManager.IsMouseOnGrid())
        {
            Vector3 mousePosition = inputManager.GetSelectedMapPosition();
            Vector3Int gridPosition = grid.WorldToCell(mousePosition);
            selectionState.OnAction(gridPosition);
        }
    }


}

/*
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