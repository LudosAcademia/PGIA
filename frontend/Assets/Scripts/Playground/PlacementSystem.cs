using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.LightTransport;

public class PlacementSystem : MonoBehaviour
{

    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;
    [SerializeField] private ObjectsDatabase database;
    [SerializeField] private GameObject gridVisualization;
    [SerializeField] private PreviewSystem previewSystem;

    IBuildingState buildingState;

    private GridData floorData, propData;
    private Vector3Int lastDetectedPosition = Vector3Int.zero;
    private bool isRemoving;
    [SerializeField] private ObjectPlacer objectPlacer;

    private void Start()
    {
        StopPlacement();
        floorData = new();
        propData = new();
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

    public void StartPlacement(int ID)
    {
        StopPlacement();
        buildingState = new PlacementState(ID, grid, previewSystem, database, floorData, propData, objectPlacer);

        gridVisualization.SetActive(true);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;

    }

    public void StartRemoving()
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        buildingState = new RemovingState(grid, previewSystem, floorData, propData, objectPlacer);
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

    private void StopPlacement()
    {
        if (buildingState == null) { return; }
        gridVisualization.SetActive(false);
        buildingState.EndState();
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
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

 */