using System;
using UnityEngine;

public class SelectState : ISelectionState
{
    PreviewSystem previewSystem;
    GridManager gridManager;
    ObjectManipulator objectManipulator;
    public static event Action<AvaItemPreBuild> OnTileItemSelect;

    private GameObject currentSelectedObject;
    private Vector3Int currentSelectedVector;
    private int currentSelectedObjectId;

    public int selectedObjectIndex = -1;

    public SelectState(PreviewSystem previewSystem, GridManager gridManager, ObjectManipulator objectManipulator)
    {
        this.previewSystem = previewSystem;
        this.gridManager = gridManager;
        this.objectManipulator = objectManipulator;

        this.previewSystem.StartShowingCursor(Vector2Int.one);
        this.previewSystem.SetCursorColor(Color.white);
    }

    public void EndState()
    {
        //previewSystem.StopShowingPreview();
        previewSystem.StopHighlightSelectedTile();

    }

    public void OnAction(Vector3Int gridPosition, string layer, GameObject selectedObject)
    {
        //previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
        //Debug.Log("Selected Pos in Grid: " + gridPosition);
        previewSystem.StartHighlightSelectedTile(gridPosition);

        if (gridManager.PlaygroundGrid.grid[layer].data[gridPosition.x, gridPosition.z].containId != -1)
        {
            //Debug.Log("Grid Base: " + gridData.gridLayers[layer][gridPosition.x, gridPosition.z].containId);
            currentSelectedObject = selectedObject;
            currentSelectedVector = gridPosition;
            currentSelectedObjectId = gridManager.PlaygroundGrid.grid[layer].data[gridPosition.x, gridPosition.z].containId;


            int currObjectId = gridManager.CurrentObjecyId;
            int selectedObjectIndex = gridManager.ObjectsDatabase.objectData.FindIndex(data => data.ID == currObjectId);
            Debug.Log(gridManager.ObjectsDatabase.objectData[selectedObjectIndex].Name);
            Debug.Log(gridManager.ObjectsDatabase.objectData[selectedObjectIndex].Interaction);

            Debug.Log("gridPosition.x, gridPosition.z " + gridPosition.x + " " + gridPosition.z);

            if (gridManager.ObjectsDatabase.objectData[selectedObjectIndex].Interaction != GameEnums.InteractType.None)
            {
                Guid instanceId = gridManager.PlaygroundGrid.grid[layer].data[gridPosition.x, gridPosition.z].instanceId;
                AvaItemPreBuild itemRef = gridManager.PlaygroundGrid.logicReadyItems[instanceId];
                OnTileItemSelect?.Invoke(itemRef);
                Debug.Log("Selected Item: " + itemRef.itemName);
            }
     
        }
        else
        {
            Debug.Log("Grid Base: empty");
        }

    }

    public void UpdateState(Vector3Int gridPosition)
    {
        //previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), true);
        previewSystem.UpdateCursor(gridPosition);
    }

    public void OnRotate(Vector3Int gridPosition)
    {
        if (currentSelectedObject != null)
        {
            objectManipulator.RotateObject(gridPosition, currentSelectedObject);
        }
        else
        {
            Debug.Log("No Object Selected");
        }
    }

    public void OnMoveStart()
    {
    }

    public void OnMoveEnd(Vector3Int gridPosition, string layer)
    {
        objectManipulator.MoveObject(currentSelectedVector, gridPosition, layer);
        previewSystem.StartHighlightSelectedTile(gridPosition);

    }

}


/*
     private int selectedObjectIndex = -1;
    int ID;
    Grid grid;
    PreviewSystem previewSystem;
    ObjectsDatabase database;
    GridData basePropData;
    GridData levelPropData;
    ObjectPlacer objectPlacer;

    public PlacementState(int iD,
                          Grid grid,
                          PreviewSystem previewSystem,
                          ObjectsDatabase database,
                          GridData basePropData,
                          GridData levelPropData,
                          ObjectPlacer objectPlacer)
    {
        ID = iD;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.basePropData = basePropData;
        this.levelPropData = levelPropData;
        this.objectPlacer = objectPlacer;


        selectedObjectIndex = database.objectData.FindIndex(data => data.ID == ID);
        if (selectedObjectIndex > -1)
        {
            //cellIndicator.SetActive(true);
            previewSystem.StartShowingPlacementPreview(database.objectData[selectedObjectIndex].Prefab,
                database.objectData[selectedObjectIndex].Size);
        }
        else
        {
            throw new System.Exception($"No Object with ID {iD}");
        }


    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {

        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        if (placementValidity == false) { return; }


        int index = objectPlacer.PlaceObject(database.objectData[selectedObjectIndex].Prefab, grid.CellToWorld(gridPosition), selectedObjectIndex);

        GridData selectedData = database.objectData[selectedObjectIndex].PropLevel == PropLevel.Base ? basePropData : levelPropData;
        selectedData.AddObject(gridPosition,
            database.objectData[selectedObjectIndex].Size,
            database.objectData[selectedObjectIndex].ID,
            index);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        GridData selectedData = database.objectData[selectedObjectIndex].PropLevel == PropLevel.Base ? basePropData : levelPropData;
        return selectedData.CanPlaceObjectAt(gridPosition, database.objectData[selectedObjectIndex].Size);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);

        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
    }
}

 
 */